using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class AreaProduceBehaviorOptions : ProduceBehaviorOptions
    {
        public AreaProduceBehaviorOptions(Agent agent, Town town, string commodity)
        {
            this.Commodity = commodity;
        }
    }


    public class ProduceBehaviorOptions
    {
        public string Commodity { get; set; }
        public double BaseAmount { get; set; }
        public double EatFactor { get; set; }
        public double ToolsFactor { get; set; }
        public double ToolsBreakChance { get; set; }
    }

    public class ProduceBehavior : AgentBehavior
    {
        private readonly Town town;
        private readonly EatBehavior eat;
        private readonly ProduceBehaviorOptions options;

        public ProduceBehavior(
            Agent agent,
            Town town,
            EatBehavior eat,
            ProduceBehaviorOptions options)
            : base(agent)
        {
            this.town = town;
            this.eat = eat;
            this.options = options;
        }

        public override void Perform()
        {
            var ratio = this.town.GetRatio(this.options.Commodity);

            var limit = ratio * this.options.BaseAmount * this.options.EatFactor * this.options.ToolsFactor;
            if (this.Agent.Inventory.Get(this.options.Commodity) < limit)
            {
                this.Agent.CostBeliefs.BeginUnit();

                var amount = this.options.BaseAmount;

                if (this.eat.Eaten)
                {
                    amount *= this.options.EatFactor;
                }

                if (0 < this.Agent.Inventory.Get(Constants.Tools))
                {
                    amount *= this.options.ToolsFactor;

                    if (this.Random.NextDouble() < this.options.ToolsBreakChance)
                    {
                        this.Consume(Constants.Tools, 1);
                    }
                }

                this.Produce(this.options.Commodity, amount);

                this.Agent.CostBeliefs.EndUnit();
            }
            else
            {
                //this.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(this.options.Commodity);
        }
    }
}
