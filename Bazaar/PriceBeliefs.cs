using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar
{
    public class PriceBeliefs
    {
        public const double MinValue = 1;
        public const double MaxValue = 100;

        private readonly Random random = new Random();
        private readonly Dictionary<string, (double, double)> priceBeliefs = new Dictionary<string, (double, double)>();

        public void Initialize(IMarket market)
        {
            foreach (var key in market.History.Keys)
            {
                var value = market.History[key];
                if (!double.IsNaN(value.AveragePrice))
                { 
                    this.priceBeliefs[key] = (value.AveragePrice, value.AveragePrice); 
                }
            }
        }

        public void Update(Offer offer)
        {
            if (offer == null) throw new ArgumentNullException(nameof(offer));

            var (minPrice, maxPrice) = this.Get(offer.Commodity);

            var newMinPrice = minPrice;
            var newMaxPrice = maxPrice;

            if (offer.Results.Any())
            {
                double price;
                var amount = offer.Results.Sum(x => x.Amount);
                if (amount != 0)
                {
                    price = offer.Results.Sum(x => x.Price * x.Amount / amount);
                }
                else
                {
                    price = offer.Results.Average(x => x.Price);
                }

                if (offer.Type == OfferType.Buy)
                {
                    newMinPrice = 0.5 * minPrice + 0.5 * (0.95 * price);
                    newMaxPrice = 0.5 * maxPrice + 0.5 * price;
                }
                else if (offer.Type == OfferType.Sell)
                {
                    newMinPrice = 0.5 * minPrice + 0.5 * price;
                    newMaxPrice = 0.5 * maxPrice + 0.5 * (1.05 * price);
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

        public double GetRandom(string commodity)
        {
            var (minPrice, maxPrice) = this.Get(commodity);
            return minPrice + this.random.NextDouble() * (maxPrice - minPrice);
        }

        public double GetAverage(string commodity)
        {
            var (minPrice, maxPrice) = this.Get(commodity);
            return (minPrice + maxPrice) / 2.0;
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
                Math.Min(Math.Max(@MinValue, minPrice), @MaxValue),
                Math.Min(Math.Max(@MinValue, maxPrice), @MaxValue)
            );
        }

        private void EnsurePriceBelief(string commodity)
        {
            if (!this.priceBeliefs.ContainsKey(commodity))
            {
                this.priceBeliefs[commodity] = (MinValue, MaxValue);
            }
        }
    }
}
