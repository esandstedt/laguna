using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Agent
{
    public class CostBeliefResult
    {
        public string Commodity { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
    }

    public class CostBeliefs
    {

        private readonly PriceBeliefs priceBeliefs;

        private Unit unit;
        private readonly Dictionary<string, List<(double, double)>> unitCosts;

        public CostBeliefs(PriceBeliefs priceBeliefs)
        {
            this.priceBeliefs = priceBeliefs;
            this.unitCosts = new Dictionary<string, List<(double, double)>>();
        }

        public void Begin()
        {
            if (this.unit != null) throw new InvalidOperationException();

            this.unit = new Unit();
        }

        public List<CostBeliefResult> End()
        {
            if (this.unit == null) throw new InvalidOperationException();

            var result = new List<CostBeliefResult>();

            var produces = this.unit.Produces;
            var consumes = this.unit.Consumes
                .GroupBy(x => x.Key)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));

            var totalCount = produces.Sum(x => x.Value);

            if (totalCount != 0 && consumes.Any())
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

                var (minTotalBelievedValue, maxTotalBelievedValue) = produces
                    .Select(pair =>
                    {
                        var (minPrice, maxPrice) = this.priceBeliefs.Get(pair.Key);
                        return (
                            pair.Value * minPrice,
                            pair.Value * maxPrice
                        );
                    })
                    .Aggregate((acc, x) => (acc.Item1 + x.Item1, acc.Item2 + x.Item2));

                var minRatio = minTotalCost / minTotalBelievedValue;
                var maxRatio = maxTotalCost / maxTotalBelievedValue;

                foreach (var commodity in produces.Keys)
                {
                    var (minPrice, maxPrice) = this.priceBeliefs.Get(commodity);

                    result.Add(new CostBeliefResult
                    {
                        Commodity = commodity,
                        MinPrice = minRatio * minPrice,
                        MaxPrice = maxRatio * maxPrice,
                    });
                }
            }

            this.unit = null;

            return result;
        }

        public void Consume(string commodity, double amount)
        {
            if (this.unit == null) throw new InvalidOperationException();

            this.unit.Consume(commodity, amount);
        }

        public void Produce(string commodity, double amount)
        {
            if (this.unit == null) throw new InvalidOperationException();

            this.unit.Produce(commodity, amount);
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
        }
    }
}
