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
        public double WorkCapacity { get; set; }
        public List<(string Commodity, double Amount)> Consumes { get; set; }
        public string Produces { get; set; }
        public double Efficiency { get; set; }
    }

    public class Industry : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }
        //public IndustryOptions Options { get; set; }
        public Dictionary<string, double> Sales { get; set; }

        private readonly List<(string Commodity, double Amount)> consumes;
        private readonly string produces;
        private readonly double capacity;

        public Industry(IndustryOptions options)
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);
            //this.Options = options;
            this.Sales = new Dictionary<string, double>();

            this.consumes = options.Consumes
                .Select(x => (x.Commodity, x.Amount / options.Efficiency))
                .ToList();
            this.produces = options.Produces;
            this.capacity = options.WorkCapacity / this.consumes.Single(x => x.Commodity == Constants.UnskilledWork).Amount;

            this.Inventory.Add(Constants.Money, 1000);
        }

        public void Step()
        {
            // Figure out how many "units" are produced
            var units = double.MaxValue;
            foreach (var consume in this.consumes)
            {
                units = Math.Min(
                    units,
                    this.Inventory.Get(consume.Commodity) / consume.Amount
                );
            }

            var soldUnits = this.Sales.GetValueOrDefault(this.produces, 0) ;
            var heldUnits = this.Inventory.Get(this.produces);

            units = Math.Min(units, Math.Max(2 * soldUnits - heldUnits, 0));

            // Produce
            this.CostBeliefs.Begin();

            foreach (var consume in this.consumes)
            {
                var amount = units * consume.Amount;
                this.CostBeliefs.Consume(consume.Commodity, amount);
                this.Inventory.Remove(consume.Commodity, amount);
            }

            {
                this.CostBeliefs.Produce(this.produces, units);
                this.Inventory.Add(this.produces, units);
            }

            this.CostBeliefs.End();

            // Ensure there's always something being produced
            if (units < 1)
            {
                this.Inventory.Add(this.produces, 1);
            }

            // Throw all unused work away
            this.Inventory.Set(Constants.UnskilledWork, 0);
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            {
                var offers = this.CreateOffers(
                    OfferType.Sell,
                    this.produces,
                    this.Inventory.Get(this.produces)
                );

                foreach (var offer in offers)
                {
                    yield return offer;
                }
            }

            {
                var offers = new List<Offer>();

                foreach (var consume in this.consumes)
                {
                    offers.AddRange(this.CreateOffers(
                        OfferType.Buy,
                        consume.Commodity,
                        this.capacity * consume.Amount
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

        public static Industry Create(
            double workCapacity,
            Recipe recipe,
            double efficiency)
        {
            return new Industry(
                new IndustryOptions
                {
                    WorkCapacity = workCapacity,
                    Consumes = recipe.Consumes,
                    Produces = recipe.Produces,
                    Efficiency = efficiency
                }
            );
        }
    }
}
