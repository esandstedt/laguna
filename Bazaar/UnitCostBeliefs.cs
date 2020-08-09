using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar
{
    public class UnitCostBeliefs
    {

        private readonly PriceBeliefs priceBeliefs;
        private readonly double minimumPrice;

        private Dictionary<string, double> consumes = new Dictionary<string, double>();
        private Dictionary<string, double> produces = new Dictionary<string, double>();
        private List<(double, double)> unitCosts = new List<(double, double)>();

        public UnitCostBeliefs(PriceBeliefs priceBeliefs, double minimumPrice)
        {
            this.priceBeliefs = priceBeliefs;
            this.minimumPrice = minimumPrice;
        }

        public void Begin()
        {
            this.consumes.Clear();
            this.produces.Clear();
        }

        public void End()
        {
            var totalCount = this.produces.Sum(x => x.Value);
            if (totalCount != 0)
            {
                var (minTotalCost, maxTotalCost) = this.consumes
                    .Select(pair =>
                    {
                        var (minPrice, maxPrice) = this.priceBeliefs.Get(pair.Key);
                        return (
                            pair.Value * minPrice,
                            pair.Value * maxPrice
                        );
                    })
                    .Aggregate((acc, x) => (acc.Item1 + x.Item1, acc.Item2 + x.Item2));

                var minUnitCost = minTotalCost / totalCount;
                var maxUnitCost = maxTotalCost / totalCount;

                this.unitCosts.Insert(0, (minUnitCost, maxUnitCost));
                if (20 < this.unitCosts.Count)
                {
                    this.unitCosts.RemoveAt(this.unitCosts.Count - 1);
                }

                var avgMinUnitCost = this.unitCosts.Average(x => x.Item1);
                var avgMaxUnitCost = this.unitCosts.Average(x => x.Item2);

                foreach (var commodity in this.produces.Keys)
                {
                    var (minPrice, maxPrice) = this.priceBeliefs.Get(commodity);

                    var newMinPrice = 0.75 * minPrice + 0.25 * (1.05 * avgMinUnitCost);
                    var newMaxPrice = 0.75 * maxPrice + 0.25 * (1.05 * avgMaxUnitCost);

                    this.priceBeliefs.Set(
                        commodity,
                        newMinPrice,
                        newMaxPrice
                    );
                }

                if (0 < minUnitCost && minUnitCost < this.minimumPrice)
                {
                    foreach (var commodity in this.consumes.Keys)
                    {
                        var (minPrice, maxPrice) = this.priceBeliefs.Get(commodity);

                        this.priceBeliefs.Set(
                            commodity,
                            this.minimumPrice * minPrice / minUnitCost,
                            this.minimumPrice * maxPrice / minUnitCost
                        );
                    }

                }
            }
        }

        public void Consume(string commodity, double amount)
        {
            if (!this.consumes.ContainsKey(commodity))
            {
                this.consumes[commodity] = 0;
            }

            this.consumes[commodity] += amount;
        }

        public void Produce(string commodity, double amount)
        {
            if (!this.produces.ContainsKey(commodity))
            {
                this.produces[commodity] = 0;
            }

            this.produces[commodity] += amount;
        }
    }
}
