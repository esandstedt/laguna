using Laguna.Agent;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class Person : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }

        public WeightedDemand FoodDemand = new WeightedDemand(
            new Dictionary<string, double>
            {
                { Constants.Fruit, 2 },
                { Constants.Vegetables, 2 },
                { Constants.Meat, 3 },
                { Constants.Fish, 3 },
                { Constants.Grain, 1 },
                { Constants.Bread, 5 },
            },
            0.10
        );

        private double Nutrition = 0;

        public Person()
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);

            this.Inventory.Add(Constants.Money, 100);
        }

        public void Step()
        {
            this.CostBeliefs.Begin();

            this.Inventory.Set(Constants.UnskilledWork, 1);
            this.CostBeliefs.Produce(Constants.UnskilledWork, 1);

            this.Nutrition = 0;
            var foodDemand = this.FoodDemand.GetDemand(this.PriceBeliefs, 1);
            foreach (var pair in foodDemand)
            {
                var (commodity, amount) = (pair.Key, pair.Value);

                this.Inventory.Set(
                    commodity,
                    Math.Max(0, this.Inventory.Get(commodity) - amount)
                );

                this.CostBeliefs.Consume(commodity, amount);

                this.Nutrition += amount;
            }

            if (0 < this.Inventory.Get(Constants.Wood))
            {
                this.Inventory.Set(
                    Constants.Wood,
                    Math.Max(0, this.Inventory.Get(Constants.Wood) - 1)
                );
                this.CostBeliefs.Consume(Constants.Wood, 1);
            }

            this.CostBeliefs.End();
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            yield return new Offer(
                OfferType.Sell,
                Constants.UnskilledWork,
                this.PriceBeliefs.GetRandom(Constants.UnskilledWork),
                1
            );


            var buyOffers = new List<Offer>();

            var foodOffers = this.FoodDemand.GenerateOffers(this.Inventory, this.PriceBeliefs, 5);
            foreach (var offer in foodOffers)
            {
                buyOffers.Add(offer);
            }

            if (0.8 < this.Nutrition)
            {
                buyOffers.Add(new Offer(
                    OfferType.Buy,
                    Constants.Wood,
                    this.PriceBeliefs.GetRandom(Constants.Wood),
                    Math.Max(2, 5 - this.Inventory.Get(Constants.Wood))
                ));
            }

            if (2 < this.Inventory.Get(Constants.Wood))
            {
                buyOffers.Add(new Offer(
                    OfferType.Buy,
                    Constants.Timber,
                    this.PriceBeliefs.GetRandom(Constants.Timber),
                    Math.Max(2, 5 - this.Inventory.Get(Constants.Timber))
                ));
            }

            var money = this.Inventory.Get(Constants.Money);
            foreach (var offer in buyOffers.SelectMany(x => x.Split(1)))
            {
                money -= offer.Price * offer.Amount;

                if (money < 0)
                {
                    break;
                }

                yield return offer;
            }
        }
    }
}
