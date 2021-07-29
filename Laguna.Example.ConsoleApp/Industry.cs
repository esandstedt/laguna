using Laguna.Agent;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{

    public class IndustryOptions
    {
        public double Capacity { get; set; }
        public string Good { get; set; }
        public double BaseRate { get; set; }
        public double Productivity { get; set; }
        public double SpoilRate { get; set; }
    }

    public class Industry : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }
        public IndustryOptions Options { get; set; }

        public Industry(IndustryOptions options)
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);
            this.Options = options;

            this.Inventory.Add(Constants.Money, 1000);
        }

        public void Step()
        {
            var workAmount = this.Inventory.Get(Constants.UnskilledWork);
            var goodAmount = this.Options.Productivity * workAmount;

            this.CostBeliefs.Begin();
            this.CostBeliefs.Consume(Constants.UnskilledWork, workAmount);
            this.CostBeliefs.Produce(this.Options.Good, goodAmount);
            this.CostBeliefs.End();

            this.Inventory.Set(this.Options.Good, (1 - this.Options.SpoilRate) * this.Inventory.Get(this.Options.Good));

            this.Inventory.Add(this.Options.Good, Math.Max(this.Options.BaseRate, goodAmount));
            this.Inventory.Set(Constants.UnskilledWork, 0);
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            var x = this.Inventory.Get(this.Options.Good);
            while (0 < x)
            {
                yield return new Offer(
                    OfferType.Sell,
                    this.Options.Good,
                    this.PriceBeliefs.GetRandom(this.Options.Good),
                    Math.Min(1, x)
                );

                x -= 1;
            }

            var buyOffers = new List<Offer>();

            x = this.Options.Capacity;
            while (0 < x)
            {
                buyOffers.Add(new Offer(
                    OfferType.Buy,
                    Constants.UnskilledWork,
                    this.PriceBeliefs.GetRandom(Constants.UnskilledWork),
                    Math.Min(1, x)
                ));

                x -= 1;
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

        public static Industry CreateFarm(double capacity)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Good = Constants.Food,
                    BaseRate = 1.0,
                    Productivity = 4.0,
                    SpoilRate = 0.25
                }
            );
        }

        public static Industry CreateForest(double capacity)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Good = Constants.Timber,
                    BaseRate = 1.0,
                    Productivity = 4.0,
                    SpoilRate = 0.05
                }
            );
        }
    }
}
