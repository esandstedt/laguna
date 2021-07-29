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
        public List<(string Commodity, double Amount)> Consumes { get; set; }
        public List<(string Commodity, double Amount)> Produces { get; set; }
        public string Good { get; set; }
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
            // Apply spoil rate to production
            foreach (var produce in this.Options.Produces)
            {
                this.Inventory.Set(
                    produce.Commodity,
                    (1 - this.Options.SpoilRate) * this.Inventory.Get(produce.Commodity)
                );
            }

            // Figure out how many "units" are produced
            var units = double.MaxValue;
            foreach (var consume in this.Options.Consumes)
            {
                units = Math.Min(
                    units,
                    this.Inventory.Get(consume.Commodity) / consume.Amount
                );
            }

            // Produce
            this.CostBeliefs.Begin();

            foreach (var consume in this.Options.Consumes)
            {
                var amount = units * consume.Amount;
                this.CostBeliefs.Consume(consume.Commodity, amount);
                this.Inventory.Remove(consume.Commodity, amount);
            }

            foreach (var produce in this.Options.Produces)
            {
                var amount = units * produce.Amount;
                this.CostBeliefs.Produce(produce.Commodity, amount);
                this.Inventory.Add(produce.Commodity, amount);
            }

            this.CostBeliefs.End();

            // Ensure there's always something being produced
            if (units < 1)
            {
                foreach (var produce in this.Options.Produces)
                {
                    this.Inventory.Add(produce.Commodity, 1);
                }
            }

            // Throw all unused work away
            this.Inventory.Set(Constants.UnskilledWork, 0);
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            foreach (var produce in this.Options.Produces)
            {
                var offers = this.CreateOffers(
                    OfferType.Sell,
                    produce.Commodity,
                    this.Inventory.Get(produce.Commodity)
                );

                foreach (var offer in offers)
                {
                    yield return offer;
                }
            }

            {
                var offers = new List<Offer>();

                foreach (var consume in this.Options.Consumes)
                {
                    offers.AddRange(this.CreateOffers(
                        OfferType.Buy,
                        consume.Commodity,
                        this.Options.Capacity * consume.Amount
                    ));
                }

                offers.Shuffle();

                var money = this.Inventory.Get(Constants.Money);
                foreach (var offer in offers)
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

        public static Industry CreateFarm(double capacity)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Consumes = new List<(string, double)>
                    {
                        (Constants.UnskilledWork, 1.0),
                    },
                    Produces = new List<(string, double)>
                    {
                        (Constants.Food, 6.0)
                    },
                    SpoilRate = 0.25,
                }
            );
        }

        public static Industry CreateForest(double capacity)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Consumes = new List<(string, double)>
                    {
                        (Constants.UnskilledWork, 1.0),
                    },
                    Produces = new List<(string, double)>
                    {
                        (Constants.Wood, 8.0)
                    },
                    SpoilRate = 0.05,
                }
            );
        }

        public static Industry CreateSawmill(double capacity)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Consumes = new List<(string Good, double Amount)>
                    {
                        (Constants.UnskilledWork, 1.0),
                        (Constants.Wood, 1.0),
                    },
                    Produces = new List<(string Good, double Amount)>
                    {
                        (Constants.Timber, 4.0)
                    },
                    SpoilRate = 0.05,
                }
            );

        }
    }
}
