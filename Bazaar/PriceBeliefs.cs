using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Initialize(Market market)
        {
            foreach (var commodity in market.GetCommodities())
            {
                IList<MarketHistory> history = market.GetHistory(commodity);
                if (history.Any())
                {
                    var list = history.Take(10).ToList();

                    var listWhereTraded = list.Where(x => 0 < x.AmountTraded).ToList();
                    if (listWhereTraded.Any())
                    {
                        var avgPrice = listWhereTraded.Average(x => x.AveragePrice);
                        this.Set(
                            commodity,
                            0.8 * avgPrice, 
                            1.2 * avgPrice
                        );
                    }
                    else
                    {
                        var avgPrice = list.Average(x => x.LowestSellingPrice);
                        this.Set(
                            commodity,
                            0.8 * avgPrice,
                            1.2 * avgPrice
                        );
                    }
                }
            }
        }

        public void Update(Offer offer)
        {
            if (offer == null) throw new ArgumentNullException(nameof(offer));
            if (offer.Result == null) throw new ArgumentException("Offer does not have a result");

            var (minPrice, maxPrice) = this.Get(offer.Commodity);

            var newMinPrice = minPrice;
            var newMaxPrice = maxPrice;

            if (offer.Result.Success)
            {
                if (offer.Type == OfferType.Buy)
                {
                    newMinPrice = 0.5 * minPrice + 0.5 * (0.95 * offer.Result.Price);
                    newMaxPrice = 0.5 * maxPrice + 0.5 * offer.Result.Price;
                }
                else if (offer.Type == OfferType.Sell)
                {
                    newMinPrice = 0.5 * minPrice + 0.5 * offer.Result.Price;
                    newMaxPrice = 0.5 * maxPrice + 0.5 * (1.05 * offer.Result.Price);
                }
            }
            else
            {
                if (offer.Type == OfferType.Buy)
                {
                    newMinPrice = minPrice;
                    newMaxPrice = 1.05 * maxPrice;
                }
                else if (offer.Type == OfferType.Sell)
                {
                    newMinPrice = 0.95 * minPrice;
                    newMaxPrice = maxPrice;
                }
            }

            this.Set(
                offer.Commodity,
                newMinPrice,
                newMaxPrice
            );

        }

        public (double, double) Get(string commodity)
        {
            if (commodity == Constants.Money)
            {
                return (1, 1);
            }

            this.EnsurePriceBelief(commodity);

            return this.priceBeliefs[commodity];
        }

        public void Set(string commodity, double minPrice, double maxPrice)
        {
            if (commodity == Constants.Money)
            {
                throw new InvalidOperationException();
            }

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
