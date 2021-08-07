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
        public (string Commodity, double Amount) Produces { get; set; }
        public string Good { get; set; }
        public double Productivity { get; set; }
        public double SpoilRate { get; set; }
    }

    public class Industry : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }
        public IndustryOptions Options { get; set; }
        public Dictionary<string, double> Sales { get; set; }

        public Industry(IndustryOptions options)
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);
            this.Options = options;
            this.Sales = new Dictionary<string, double>();

            this.Inventory.Add(Constants.Money, 1000);
        }

        public void Step()
        {
            // Apply spoil rate to production
            this.Inventory.Set(
                this.Options.Produces.Commodity,
                (1 - this.Options.SpoilRate) * this.Inventory.Get(this.Options.Produces.Commodity)
            );

            // Figure out how many "units" are produced
            var units = double.MaxValue;
            foreach (var consume in this.Options.Consumes)
            {
                units = Math.Min(
                    units,
                    this.Inventory.Get(consume.Commodity) / consume.Amount
                );
            }

            var soldUnits = this.Sales.GetValueOrDefault(this.Options.Produces.Commodity, 0) / this.Options.Produces.Amount;
            var heldUnits = this.Inventory.Get(this.Options.Produces.Commodity) / this.Options.Produces.Amount;

            units = Math.Min(units, Math.Max(2 * soldUnits - heldUnits, 0));

            // Produce
            this.CostBeliefs.Begin();

            foreach (var consume in this.Options.Consumes)
            {
                var amount = units * consume.Amount;
                this.CostBeliefs.Consume(consume.Commodity, amount);
                this.Inventory.Remove(consume.Commodity, amount);
            }

            {
                var amount = units * this.Options.Produces.Amount;
                this.CostBeliefs.Produce(this.Options.Produces.Commodity, amount);
                this.Inventory.Add(this.Options.Produces.Commodity, amount);
            }

            this.CostBeliefs.End();

            // Ensure there's always something being produced
            if (units < 1)
            {
                this.Inventory.Add(this.Options.Produces.Commodity, 1);
            }

            // Throw all unused work away
            this.Inventory.Set(Constants.UnskilledWork, 0);
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            {
                var offers = this.CreateOffers(
                    OfferType.Sell,
                    this.Options.Produces.Commodity,
                    this.Inventory.Get(this.Options.Produces.Commodity)
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

        public override void HandleOfferResults(IEnumerable<Offer> offers)
        {
            base.HandleOfferResults(offers);

            this.Sales.Clear();
            foreach (var offer in offers.Where(x => x.Type == OfferType.Sell && x.Results.Any()))
            {
                if (!this.Sales.ContainsKey(offer.Commodity))
                {
                    this.Sales[offer.Commodity] = 0;
                }

                this.Sales[offer.Commodity] += offer.Results.Sum(x => x.Amount);
            }
        }

        public static Industry CreateRaw(
            double capacity,
            string producesCommodity,
            double producesAmount,
            double spoilRate)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Consumes = new List<(string, double)>
                    {
                        (Constants.UnskilledWork, 1.0),
                    },
                    Produces = (producesCommodity, producesAmount),
                    SpoilRate = spoilRate 
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
                    Produces = (Constants.Timber, 4.0),
                    SpoilRate = 0.05,
                }
            );
        }

        public static Industry CreateBakery(double capacity)
        {
            return new Industry(
                new IndustryOptions
                {
                    Capacity = capacity,
                    Consumes = new List<(string Good, double Amount)>
                    {
                        (Constants.UnskilledWork, 1.0),
                        (Constants.Grain, 1.0),
                    },
                    Produces = (Constants.Bread, 4.0),
                    SpoilRate = 0.125,
                }
            );
        }
    }
}
