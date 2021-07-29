using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Agent
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

        public void Update(IEnumerable<Offer> offers)
        {
            if (offers == null) throw new ArgumentNullException(nameof(offers));

            var groups = offers
                .GroupBy(x => x.Commodity)
                .Select(g => new {
                    Commodity = g.Key,
                    Offers = g.ToList()
                });

            foreach (var group in groups)
            {
                var commodity = group.Commodity;

                var (minPrice, maxPrice) = this.Get(commodity);

                var newMinPrices = new List<double>();
                var newMaxPrices = new List<double>();

                foreach (var offer in group.Offers)
                {
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

                        var ratio = 0.9;

                        if (offer.Type == OfferType.Buy)
                        {
                            newMinPrice = 0.975 * (ratio * minPrice + (1 - ratio) * price);
                            newMaxPrice = 1.000 * (ratio * maxPrice + (1 - ratio) * price);
                        }
                        else if (offer.Type == OfferType.Sell)
                        {
                            newMinPrice = 1.000 * (ratio * minPrice + (1 - ratio) * price);
                            newMaxPrice = 1.025 * (ratio * maxPrice + (1 - ratio) * price);
                        }
                    }
                    else
                    {
                        if (offer.Type == OfferType.Buy)
                        {
                            newMinPrice = 1.025 * minPrice;
                            newMaxPrice = 1.050 * maxPrice;
                        }
                        else if (offer.Type == OfferType.Sell)
                        {
                            newMinPrice = 0.950 * minPrice;
                            newMaxPrice = 0.975 * maxPrice;
                        }
                    }

                    newMinPrices.Add(newMinPrice);
                    newMaxPrices.Add(newMaxPrice);
                }

                this.Set(
                    commodity,
                    newMinPrices.Average(),
                    newMaxPrices.Average()
                );
            }

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
