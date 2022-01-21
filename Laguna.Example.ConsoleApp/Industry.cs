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
        public List<Recipe> Recipes { get; set; }
        public double Efficiency { get; set; }
        public bool Debug { get; set; }
    }

    public class Industry : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }
        public Dictionary<string, double> Sales { get; set; }

        private List<Recipe> recipes;
        private readonly double capacity;
        private readonly double efficiency;
        private readonly bool debug;

        private Dictionary<Recipe, double> produces;
        private Dictionary<string, double> consumes;

        public Industry(IndustryOptions options)
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);
            this.Sales = new Dictionary<string, double>();

            this.recipes = options.Recipes;
            this.capacity = options.WorkCapacity;
            this.efficiency = options.Efficiency;
            this.debug = options.Debug;

            this.produces = new Dictionary<Recipe, double>();
            this.consumes = new Dictionary<string, double>();

            this.Inventory.Add(Constants.Money, 1000);
        }


        public void Step()
        {
            if (this.debug)
            {
                { }
            }

            // Produce

            var producedUnits = new Dictionary<string, double>();
            foreach (var x in this.recipes)
            {
                producedUnits[x.Produces] = 0;
            }

            foreach (var produces in this.produces)
            {
                var recipe = produces.Key;

                var units = produces.Value;
                foreach (var consume in recipe.Consumes)
                {
                    units = Math.Min(
                        units,
                        this.Inventory.Get(consume.Commodity) / consume.Amount
                    );
                }

                this.CostBeliefs.Begin();

                foreach (var consume in recipe.Consumes)
                {
                    var amount = units * consume.Amount;
                    this.CostBeliefs.Consume(consume.Commodity, amount);
                    this.Inventory.Remove(consume.Commodity, amount);
                }

                {
                    var amount = units * this.efficiency;
                    this.CostBeliefs.Produce(recipe.Produces, amount);
                    this.Inventory.Add(recipe.Produces, amount);

                    producedUnits[recipe.Produces] += amount;
                }

                var results = this.CostBeliefs.End();

                foreach (var result in results)
                {
                    var (minPrice, maxPrice) = this.PriceBeliefs.Get(result.Commodity);

                    this.PriceBeliefs.Set(
                        result.Commodity,
                        0.5 * minPrice + 0.5 * result.MinPrice,
                        0.5 * maxPrice + 0.5 * result.MaxPrice
                    );
                }
            }

            // Throw all unused work away
            this.Inventory.Set(Constants.UnskilledWork, 0);

            this.UpdateStrategy(producedUnits);
        }

        private void UpdateStrategy(Dictionary<string, double> producedUnits)
        {
            // Determine what to stock and in what quantities.
            var goal = this.recipes
                .Select(recipe =>
                {
                    var sales = this.Sales.GetValueOrDefault(recipe.Produces, 0);
                    var amount = Math.Max(1, 2 * sales);
                    return (recipe, amount);
                })
                .ToDictionary(x => x.Item1, x => x.Item2);

            // Determine what to produce to meet the goal.
            var produces = goal
                .Select(pair =>
                {
                    var recipe = pair.Key;
                    var amount = pair.Value;

                    var diff = amount - (this.Inventory.Get(recipe.Produces) - producedUnits[recipe.Produces]); 

                    return (recipe, Math.Max(0, diff));
                })
                .ToDictionary(x => x.Item1, x => x.Item2);

            // Restrict production to within capacity.
            var ratio = Math.Min(1.0, this.capacity / produces.Sum(x => x.Value));
            this.produces = produces
                .ToDictionary(x => x.Key, x => ratio * x.Value);

            // Determine what is needed to satisfy production.
            this.consumes = this.produces
                .SelectMany(x =>x.Key.Consumes.Select(y => (y.Commodity, x.Value * y.Amount)))
                .GroupBy(x => x.Item1)
                .Select(g =>
                {
                    var amount = 1.25 * g.Sum(x => x.Item2);
                    var diff = amount - this.Inventory.Get(g.Key);

                    return (g.Key, Math.Max(0, diff));
                })
                .ToDictionary(x => x.Item1, x => x.Item2);

            if (this.debug)
            {
                { }
            }
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            foreach (var recipe in this.recipes) 
            { 
                var offers = this.CreateOffers(
                    OfferType.Sell,
                    recipe.Produces,
                    this.Inventory.Get(recipe.Produces)
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
                    var commodity = consume.Key;
                    var amount = consume.Value;

                    offers.AddRange(this.CreateOffers(
                        OfferType.Buy,
                        consume.Key,
                        consume.Value
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
            List<Recipe> recipes,
            double efficiency,
            bool debug = false)
        {
            return new Industry(
                new IndustryOptions
                {
                    WorkCapacity = workCapacity,
                    Recipes = recipes,
                    Efficiency = efficiency,
                    Debug = debug
                }
            );
        }
    }
}
