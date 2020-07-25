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

        private static readonly double MINIMUM_PRICE = 1;
        private static readonly double MAXIMUM_PRICE = 100;

        public string Type { get; }
        public List<AgentBehavior> Behaviors = new List<AgentBehavior>();
        public Inventory Inventory { get; set; } = new Inventory();
        public PriceBeliefs PriceBeliefs = new PriceBeliefs(MINIMUM_PRICE, MAXIMUM_PRICE);

        private Random random = new Random();
        private List<(string, double)> buys = new List<(string, double)>();
        private List<string> sells = new List<string>();
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
                        .ToList();

                    var listWhereTraded = list.Where(x => 0 < x.AmountTraded).ToList();
                    if (listWhereTraded.Any())
                    {
                        var avgPrice = listWhereTraded.Average(x => x.AveragePrice);
                        this.PriceBeliefs.Set(
                            commodity,
                            0.8 * avgPrice, 
                            1.2 * avgPrice
                        );
                    }
                    else
                    {
                        var avgPrice = list.Average(x => x.LowestSellingPrice);
                        this.PriceBeliefs.Set(
                            commodity,
                            0.8 * avgPrice,
                            1.2 * avgPrice
                        );
                    }
                }
            }
        }

        private List<(double, double)> unitCosts = new List<(double, double)>();

        public virtual void Step()
        {
            this.consumes.Clear();
            this.produces.Clear();

            foreach (var behavior in this.Behaviors)
            {
                behavior.Perform();
            }

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

                    var newMinPrice = 0.75 * minPrice + 0.25 * (1.05 * avgMinUnitCost);
                    var newMaxPrice = 0.75 * maxPrice + 0.25 * (1.05 * avgMaxUnitCost);

                    this.PriceBeliefs.Set(
                        commodity,
                        newMinPrice,
                        newMaxPrice
                    );
                }

                if (0 < minUnitCost && minUnitCost < MINIMUM_PRICE)
                {
                    foreach (var commodity in this.consumes.Keys)
                    {
                        var (minPrice, maxPrice) = this.PriceBeliefs.Get(commodity);

                        this.PriceBeliefs.Set(
                            commodity,
                            MINIMUM_PRICE * minPrice / minUnitCost,
                            MINIMUM_PRICE * maxPrice / minUnitCost
                        );
                    }

                }
            }

        }

        public IEnumerable<Offer> GenerateOffers()
        {
            var money = this.Inventory.Get("money");

            var offers = this.Behaviors
                .SelectMany(x => x.GenerateOffers())
                .ToList();

            var buyOffers = offers
                .Where(x => x.Type == OfferType.Buy)
                .SelectMany(x => this.SplitOffer(x, 1));

            foreach (var offer in buyOffers)
            {
                money -= offer.Price * offer.Amount;

                if (money < 0)
                {
                    break;
                }

                yield return offer;
            }

            foreach (var offer in offers.Where(x => x.Type == OfferType.Sell))
            {
                yield return offer;
            }

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

        private IEnumerable<Offer> SplitOffer(Offer offer, double splitAmount)
        {
            var amount = offer.Amount;
            while (splitAmount < amount)
            {
                amount -= splitAmount;

                var clone = offer.Clone();
                clone.Amount = splitAmount;
                yield return clone;
            }

            {
                var clone = offer.Clone();
                clone.Amount = amount;
                yield return clone;
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

            var newMinPrice = minPrice;
            var newMaxPrice = maxPrice;

            if (success)
            {
                if (type == OfferType.Buy)
                {
                    newMinPrice = 0.5 * minPrice + 0.5 * (0.95 * price);
                    newMaxPrice = 0.5 * maxPrice + 0.5 * price;
                }
                else if (type == OfferType.Sell)
                {
                    newMinPrice = 0.5 * minPrice + 0.5 * price;
                    newMaxPrice = 0.5 * maxPrice + 0.5 * (1.05 * price);
                }
            }
            else
            {
                if (type == OfferType.Buy)
                {
                    newMinPrice = minPrice;
                    newMaxPrice = Math.Min(1.05 * maxPrice, money);
                }
                else if (type == OfferType.Sell)
                {
                    newMinPrice = 0.95 * minPrice;
                    newMaxPrice = maxPrice;
                }
            }

            this.PriceBeliefs.Set(
                commodity,
                newMinPrice,
                newMaxPrice
            );
        }

        protected void Buys(string commodity, int amount)
        {
            this.buys.Add((commodity, amount));
        }

        protected void Sells(string commodity)
        {
            this.sells.Add(commodity);
        }

        public void Consume(string commodity, double amount)
        {
            this.Inventory.Remove(commodity, amount);

            if (!this.consumes.ContainsKey(commodity))
            {
                this.consumes[commodity] = 0;
            }

            this.consumes[commodity] += amount;
        }

        public void Produce(string commodity, double amount)
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
