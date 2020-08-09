using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar
{
    public class CostBeliefs
    {

        private readonly PriceBeliefs priceBeliefs;
        private readonly double minimumPrice;

        private Unit baseUnit;
        private Unit currentUnit;
        private readonly List<Unit> units;
        private readonly Dictionary<string, List<(double, double)>> unitCosts;

        public CostBeliefs(PriceBeliefs priceBeliefs, double minimumPrice)
        {
            this.priceBeliefs = priceBeliefs;
            this.minimumPrice = minimumPrice;

            this.baseUnit = new Unit();
            this.units = new List<Unit>();
            this.unitCosts = new Dictionary<string, List<(double, double)>>();
        }

        public void Begin()
        {
            this.baseUnit.Clear();
            this.currentUnit = null;
            this.units.Clear();
        }

        public void End()
        {
            foreach (var unit in this.units)
            {
                var produces = unit.Produces;
                var consumes = unit.Consumes
                    .Concat(baseUnit.Consumes)
                    .GroupBy(x => x.Key)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));

                var totalCount = produces.Sum(x => x.Value);

                if (totalCount != 0)
                {
                    var (minTotalCost, maxTotalCost) = consumes
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

                    foreach (var commodity in produces.Keys)
                    {
                        if (!this.unitCosts.ContainsKey(commodity))
                        {
                            this.unitCosts[commodity] = new List<(double, double)>();
                        }
                        
                        this.unitCosts[commodity].Insert(0, (minUnitCost, maxUnitCost));
                        if (20 < this.unitCosts.Count)
                        {
                            this.unitCosts[commodity].RemoveAt(this.unitCosts.Count - 1);
                        }

                        var avgMinUnitCost = this.unitCosts[commodity].Average(x => x.Item1);
                        var avgMaxUnitCost = this.unitCosts[commodity].Average(x => x.Item2);

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
                        foreach (var commodity in consumes.Keys)
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
        }

        public void BeginUnit()
        {
            if (this.currentUnit != null)
            {
                throw new InvalidOperationException();
            }

            this.currentUnit = new Unit();
            this.units.Add(this.currentUnit);
        }

        public void EndUnit()
        {
            this.currentUnit = null;
        }

        public void Consume(string commodity, double amount)
        {
            if (this.currentUnit != null)
            {
                this.currentUnit.Consume(commodity, amount);
            }
            else
            {
                this.baseUnit.Consume(commodity, amount);
            }
        }

        public void Produce(string commodity, double amount)
        {
            if (this.currentUnit != null)
            {
                this.currentUnit.Produce(commodity, amount);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private class Unit
        {
            public readonly Dictionary<string, double> Consumes = new Dictionary<string, double>();
            public readonly Dictionary<string, double> Produces = new Dictionary<string, double>();

            public void Consume(string commodity, double amount)
            {
                if (!this.Consumes.ContainsKey(commodity))
                {
                    this.Consumes[commodity] = 0;
                }

                this.Consumes[commodity] += amount;
            }

            public void Produce(string commodity, double amount)
            {
                if (!this.Produces.ContainsKey(commodity))
                {
                    this.Produces[commodity] = 0;
                }

                this.Produces[commodity] += amount;
            }

            public void Clear()
            {
                this.Consumes.Clear();
                this.Produces.Clear();
            }
        }
    }
}
