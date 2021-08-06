using Laguna.Agent;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class WeightedDemand
    {
        private readonly Dictionary<string, double> weights;
        private readonly double threshold;

        public WeightedDemand(Dictionary<string, double> weights, double threshold)
        {
            this.weights = weights;
            this.threshold = threshold;
        }

        public IEnumerable<Offer> GenerateOffers(Inventory inventory, PriceBeliefs priceBeliefs, double totalDemand)
        {
            var demand = this.GetDemand(priceBeliefs, totalDemand);
            foreach (var pair in demand)
            {
                var (commodity, amount) = (pair.Key, pair.Value);
                var current = inventory.Get(commodity);

                yield return new Offer(
                    OfferType.Buy,
                    commodity,
                    priceBeliefs.GetRandom(commodity),
                    Math.Max(0, amount - current)
                );
            }
        }

        public Dictionary<string, double> GetDemand(PriceBeliefs priceBeliefs, double totalDemand)
        {
            var list = this.GetFavorabilities(priceBeliefs)
                .Select(pair => new
                {
                    Commodity = pair.Key,
                    Demand = (this.threshold < pair.Value) ? pair.Value : 0
                })
                .ToList();

            var ratio = 1 / list.Sum(x => x.Demand);

            return list.ToDictionary(
                x => x.Commodity,
                x => ratio * x.Demand
            );
        }

        private Dictionary<string, double> GetFavorabilities(PriceBeliefs priceBeliefs)
        {
            var list = this.weights
                .Select(pair =>
                {
                    var (commodity, weight) = (pair.Key, pair.Value);
                    var price = priceBeliefs.Get(commodity).Item2;
                    return new
                    {
                        Commodity = commodity,
                        Value = weight / price
                    };
                })
                .ToList();

            var totalValue = list.Sum(x => x.Value);

            return list.ToDictionary(
                x => x.Commodity,
                x => x.Value / totalValue
            );
        }
    }
}
