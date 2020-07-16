using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
    public class PriceBeliefs
    {
        private Dictionary<string, (double, double)> priceBeliefs = new Dictionary<string, (double, double)>();

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
            this.priceBeliefs[commodity] = (minPrice, maxPrice);
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
