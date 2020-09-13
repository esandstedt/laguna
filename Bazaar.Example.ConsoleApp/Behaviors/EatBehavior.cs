using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class EatBehavior : AgentBehavior
    {
        private static readonly double NUTRITION_DAILY = 1.25;
        private static readonly double NUTRITION_EATEN = 1.0;
        private static readonly double FAVORABILITY_THRESHOLD = 0.1;

        private static readonly Dictionary<string, double> COMMODITY_NUTRITION = new Dictionary<string, double>
        {
            { Constants.Bread, 1.25 },
            { Constants.Fish, 0.75 },
            { Constants.Apples, 0.5 },
            { Constants.Oranges, 0.5 },
        };

        public bool Eaten { get; private set; }

        private Dictionary<string, double> favorability;

        public EatBehavior(Agent agent) : base(agent) 
        {
            this.Agent.Inventory.Add(Constants.Bread, 2);
        }

        public override void Perform()
        {
            this.UpdateFavorability();

            var totalNutrition = 0.0;

            foreach (var pair in this.favorability)
            {
                var (commodity, percent) = (pair.Key, pair.Value); 
                var nutrition = COMMODITY_NUTRITION[commodity];

                var amount = Math.Min(
                    percent * NUTRITION_DAILY / nutrition,
                    this.Agent.Inventory.Get(commodity)
                );

                if (0 < amount)
                {
                    this.Agent.Consume(commodity, amount);
                    totalNutrition += amount * nutrition;
                }
            }

            foreach (var pair in this.favorability.OrderByDescending(x => x.Value))
            {
                var commodity = pair.Key;

                if (NUTRITION_EATEN <= totalNutrition)
                {
                    break;
                }

                var nutrition = COMMODITY_NUTRITION[commodity];

                var amount = Math.Min(
                    (NUTRITION_DAILY - totalNutrition) / nutrition,
                    this.Agent.Inventory.Get(commodity)
                );

                if (0 < amount)
                {
                    this.Agent.Consume(commodity, amount);
                    totalNutrition += amount * nutrition;
                }
            }

            this.Eaten = NUTRITION_EATEN < totalNutrition;

            if (!this.Eaten)
            {
                this.Agent.Consume("money", 1);
            }
        }

        private void UpdateFavorability()
        {
            var list = COMMODITY_NUTRITION
                .Select(pair =>
                {
                    var (commodity, nutrition) = (pair.Key, pair.Value);
                    var belief = this.Agent.PriceBeliefs.Get(commodity);
                    var price = belief.Item2;
                    return new
                    {
                        Commodity = commodity,
                        Value = nutrition / price
                    };
                })
                .ToList();

            var totalValue = list.Sum(x => x.Value);
            this.favorability = list.ToDictionary(x => x.Commodity, x => x.Value / totalValue);
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            foreach (var pair in this.favorability)
            {
                var (commodity, percent) = (pair.Key, pair.Value);

                double amount = 0;
                if (FAVORABILITY_THRESHOLD < percent)
                {
                    amount = 2 * percent * NUTRITION_DAILY / COMMODITY_NUTRITION[commodity];
                }

                yield return this.Buy(commodity, amount);
            }
        }
    }
}
