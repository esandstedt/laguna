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
        public Recipe Recipe { get; set; }
        public double Efficiency { get; set; }
    }

    public class Industry : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }
        public Dictionary<string, double> Sales { get; set; }

        private Recipe recipe;
        private readonly double capacity;
        private readonly double efficiency;

        public Industry(IndustryOptions options)
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);
            this.Sales = new Dictionary<string, double>();

            this.recipe = options.Recipe;
            this.capacity = options.WorkCapacity;
            this.efficiency = options.Efficiency;

            this.Inventory.Add(Constants.Money, 1000);
        }

        public void Step()
        {
            // Figure out how many "units" are produced
            var units = double.MaxValue;
            foreach (var consume in this.recipe.Consumes)
            {
                units = Math.Min(
                    units,
                    this.Inventory.Get(consume.Commodity) / consume.Amount
                );
            }

            var soldUnits = this.Sales.GetValueOrDefault(this.recipe.Produces, 0) ;
            var heldUnits = this.Inventory.Get(this.recipe.Produces);

            units = Math.Min(units, Math.Max(2 * soldUnits - heldUnits, 0));

            // Produce
            this.CostBeliefs.Begin();

            foreach (var consume in this.recipe.Consumes)
            {
                var amount = units * consume.Amount;
                this.CostBeliefs.Consume(consume.Commodity, amount);
                this.Inventory.Remove(consume.Commodity, amount);
            }

            {
                var amount = units * this.efficiency;
                this.CostBeliefs.Produce(this.recipe.Produces, amount);
                this.Inventory.Add(this.recipe.Produces, amount);
            }

            this.CostBeliefs.End();

            // Ensure there's always something being produced
            if (units < 1)
            {
                this.Inventory.Add(this.recipe.Produces, this.efficiency);
            }

            // Throw all unused work away
            this.Inventory.Set(Constants.UnskilledWork, 0);
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            {
                var offers = this.CreateOffers(
                    OfferType.Sell,
                    this.recipe.Produces,
                    this.Inventory.Get(this.recipe.Produces)
                );

                foreach (var offer in offers)
                {
                    yield return offer;
                }
            }

            {
                var offers = new List<Offer>();

                foreach (var consume in this.recipe.Consumes)
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
                    Recipe = recipe,
                    Efficiency = efficiency
                }
            );
        }
    }
}
