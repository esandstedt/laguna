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
                { Constants.RawFood, 1 },
                { Constants.Food, 10 },
            },
            0.05
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

            {
                var amount = Math.Clamp(this.Nutrition, 0.5, 1);
                this.Inventory.Set(Constants.UnskilledWork, amount);

                this.CostBeliefs.Produce(Constants.UnskilledWork, 1);
            }

            if (0 < this.Inventory.Get(Constants.Wood))
            {
                this.Inventory.Set(
                    Constants.Wood,
                    Math.Max(0, this.Inventory.Get(Constants.Wood) - 0.5)
                );

                this.CostBeliefs.Consume(Constants.Wood, 0.5);
            }

            var results = this.CostBeliefs.End();

            foreach (var result in results)
            {
                var (minPrice, maxPrice) = this.PriceBeliefs.Get(result.Commodity);

                this.PriceBeliefs.Set(
                    result.Commodity,
                    0.995 * minPrice + 0.005 * Math.Max(minPrice, result.MinPrice),
                    0.995 * maxPrice + 0.005 * Math.Max(maxPrice, result.MaxPrice)
                );
            }
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

            buyOffers.Add(new Offer(
                OfferType.Buy,
                Constants.Wood,
                this.PriceBeliefs.GetRandom(Constants.Wood),
                Math.Clamp(4 - this.Inventory.Get(Constants.Wood), 0, 2)
            ));

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
