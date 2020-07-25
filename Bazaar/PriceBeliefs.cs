using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
    public class PriceBeliefs
    {
        private readonly double minimum;
        private readonly double maximum;

        private readonly Dictionary<string, (double, double)> priceBeliefs = new Dictionary<string, (double, double)>();

        public PriceBeliefs(double minimum, double maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }

        public (double, double) Get(string commodity)
        {
            if (commodity == "money")
            {
                return (1, 1);
            }

            this.EnsurePriceBelief(commodity);

            return this.priceBeliefs[commodity];
        }

        public void Set(string commodity, double minPrice, double maxPrice)
        {
            this.priceBeliefs[commodity] = (
                Math.Min(Math.Max(this.minimum, minPrice), this.maximum),
                Math.Min(Math.Max(this.minimum, maxPrice), this.maximum)
            );
        }

        private void EnsurePriceBelief(string commodity)
        {
            if (!this.priceBeliefs.ContainsKey(commodity))
            {
                this.priceBeliefs[commodity] = (1, 1);
            }
        }
    }
}
