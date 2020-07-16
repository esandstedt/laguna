using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;

namespace Bazaar
{
    public abstract class Agent
    {

        public string Type { get; }
        public Inventory Inventory { get; set; } = new Inventory();

        private Random random = new Random();

        private List<(string, double)> buys = new List<(string, double)>();
        private List<string> sells = new List<string>();

        public PriceBeliefs PriceBeliefs = new PriceBeliefs();

        private Dictionary<string, double> consumes = new Dictionary<string, double>();
        private Dictionary<string, double> produces = new Dictionary<string, double>();

        public Agent(Market market, string type)
        {
            this.Type = type;
            this.InitializePriceBeliefs(market);
        }

        private void InitializePriceBeliefs(Market market)
        {
            foreach (var commodity in market.History.Keys)
            {
                IList<MarketHistory> history = market.History.GetValueOrDefault(commodity);
                if (history != null)
                {
                    var list = history.Reverse()
                        .Take(10)
                        .Where(x => x.AmountTraded != 0)
                        .ToList();

                    if (list.Any())
                    {
                        var avgPrice = list.Sum(x => x.AveragePrice) / list.Count;
                        this.PriceBeliefs.Set(commodity, 0.8 * avgPrice, 1.2 * avgPrice);
                    }
                }
            }
        }

        private List<(double, double)> unitCosts = new List<(double, double)>();

        public virtual void Step()
        {
            this.consumes.Clear();
            this.produces.Clear();

            this.PerformProduction();

            var totalCount = this.produces.Sum(x => x.Value);
            if (totalCount != 0)
            {
                var (minTotalCost, maxTotalCost) = this.consumes
                    .Select(pair =>
                    {
                        var (minPrice, maxPrice) = this.PriceBeliefs.Get(pair.Key);
                        return (
                            pair.Value * minPrice,
                            pair.Value * maxPrice
                        );
                    })
                    .Aggregate((acc, x) => (acc.Item1 + x.Item1, acc.Item2 + x.Item2));

                var minUnitCost = minTotalCost / totalCount;
                var maxUnitCost = maxTotalCost / totalCount;

                this.unitCosts.Add((minUnitCost, maxUnitCost));
                if (20 < this.unitCosts.Count)
                {
                    this.unitCosts.RemoveAt(0);
                }

                var avgMinUnitCost = this.unitCosts.Average(x => x.Item1);
                var avgMaxUnitCost = this.unitCosts.Average(x => x.Item2);

                foreach (var commodity in this.produces.Keys)
                {
                    var (minPrice, maxPrice) = this.PriceBeliefs.Get(commodity);
                    var newMinPrice = minPrice < avgMinUnitCost ? 0.25 * minPrice + 0.75 * avgMinUnitCost : minPrice;
                    var newMaxPrice = maxPrice < avgMaxUnitCost ? 0.25 * maxPrice + 0.75 * avgMaxUnitCost : maxPrice;
                    this.PriceBeliefs.Set(commodity, newMinPrice, newMaxPrice);
                    /*
                    if (minPrice < unitCost)
                    {
                        this.PriceBeliefs.Set(
                            commodity,
                            0.5 * minPrice + 0.5 * unitCost,
                            unitCost < maxPrice ? maxPrice : 0.25 * maxPrice + 0.75 * unitCost
                        );
                    }
                     */
                }
            }

        }

        protected abstract void PerformProduction();

        public IEnumerable<Offer> GenerateOffers()
        {
            var money = this.Inventory.Get("money");

            foreach (var (commodity, desiredAmount) in this.buys)
            {
                var amount = this.Inventory.Get(commodity);
                if (amount < desiredAmount)
                {
                    var offer = this.CreateBuyOffer(commodity, desiredAmount - amount, money);
                    if (offer != null)
                    {
                        money -= offer.Amount * offer.Price;
                        yield return offer;
                    }
                }
            }

            foreach (var commodity in this.sells)
            {
                var amount = this.Inventory.Get(commodity);
                if (0 < amount)
                {
                    yield return this.CreateSellOffer(commodity, amount);
                }
            }
        }

        private Offer CreateBuyOffer(string commodity, double amount, double money)
        {
            var (minPrice, maxPrice) = this.PriceBeliefs.Get(commodity);
            var price = minPrice + this.random.NextDouble() * (maxPrice - minPrice);

            amount = Math.Min(amount, Math.Floor(money / price));

            return new Offer
            {
                Agent = this,
                Type = OfferType.Buy,
                Commodity = commodity,
                Amount = amount,
                Price = price
            };
        }

        private Offer CreateSellOffer(string commodity, double amount)
        {
            var (minPrice, maxPrice) = this.PriceBeliefs.Get(commodity);
            var price = minPrice + this.random.NextDouble() * (maxPrice - minPrice);

            return new Offer
            {
                Agent = this,
                Type = OfferType.Sell,
                Commodity = commodity,
                Amount = amount,
                Price = price
            };
        }


        public void UpdatePriceModel(OfferType type, string commodity, bool success, double price = 0)
        {
            var (minPrice, maxPrice) = this.PriceBeliefs.Get(commodity);
            var money = this.Inventory.Get("money");

            if (success)
            {
                if (price < minPrice)
                {
                    this.PriceBeliefs.Set(
                        commodity,
                        0.90 * minPrice + 0.10 * price,
                        0.95 * maxPrice + 0.05 * price
                    );
                }
                else if (maxPrice < price)
                {
                    this.PriceBeliefs.Set(
                        commodity,
                        0.95 * minPrice + 0.05 * price,
                        0.90 * maxPrice + 0.10 * price
                    );
                }
                else
                {
                    this.PriceBeliefs.Set(
                        commodity,
                        0.95 * minPrice + 0.05 * price,
                        0.95 * maxPrice + 0.05 * price
                    );
                }
            }
            else
            {
                if (type == OfferType.Buy)
                {
                    this.PriceBeliefs.Set(
                        commodity,
                        minPrice,
                        Math.Min(1.05 * maxPrice, money)
                    );
                }
                else if (type == OfferType.Sell)
                {
                    this.PriceBeliefs.Set(
                        commodity,
                        0.95 * minPrice,
                        maxPrice
                    );
                }
            }
        }

        protected void Buys(string commodity, int amount)
        {
            this.buys.Add((commodity, amount));
        }

        protected void Sells(string commodity)
        {
            this.sells.Add(commodity);
        }

        protected void Consume(string commodity, double amount)
        {
            this.Inventory.Remove(commodity, amount);

            if (!this.consumes.ContainsKey(commodity))
            {
                this.consumes[commodity] = 0;
            }

            this.consumes[commodity] += amount;
        }

        protected void Produce(string commodity, double amount)
        {
            this.Inventory.Add(commodity, amount);

            if (!this.produces.ContainsKey(commodity))
            {
                this.produces[commodity] = 0;
            }

            this.produces[commodity] += amount;
        }

    }
}
