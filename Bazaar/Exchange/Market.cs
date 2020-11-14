using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bazaar.Exchange
{
    public interface IMarket
    {
        void AddOffer(Offer offer);
        void ResolveOffers();

        IEnumerable<MarketHistory> GetHistory(string commodity);
        IEnumerable<string> GetCommodities();
    }

    public class Market : IMarket
    {

        private const int HISTORY_LIMIT = 20;

        private readonly List<Offer> offers = new List<Offer>();
        private readonly Dictionary<string, List<MarketHistory>> history = new Dictionary<string, List<MarketHistory>>();

        public void AddOffer(Offer offer)
        {
            if (offer == null) throw new ArgumentNullException(nameof(offer));

            this.offers.Add(offer);
        }

        public void ResolveOffers()
        {
            var amountRemaining = this.offers.ToDictionary(x => x, x => x.Amount);

            foreach (var group in this.offers.GroupBy(x => x.Commodity))
            {
                var commodity = group.Key;

                var buys = new Stack<Offer>(
                    group
                        .Where(x => x.Type == OfferType.Buy)
                        .OrderBy(x => x.Price)
                        .ToList()
                );

                var sells = new Stack<Offer>(
                    group
                        .Where(x => x.Type == OfferType.Sell)
                        .OrderByDescending(x => x.Price)
                        .ToList()
                );

                { }

                int succesfulTrades = 0;
                double moneyTraded = 0;
                double amountTraded = 0;
                double amountToBuy = buys.Sum(x => x.Amount);
                double amountToSell = sells.Sum(x => x.Amount);
                double lowestSellingPrice = sells.Any() ? sells.Min(x => x.Price) : 1;
                double highestSellingPrice = sells.Any() ? sells.Max(x => x.Price) : 1;
                double lowestBuyingPrice = buys.Any() ? buys.Min(x => x.Price) : 1;
                double highestBuyingPrice = buys.Any() ? buys.Max(x => x.Price) : 1;

                while (buys.Count != 0 && sells.Count != 0)
                {
                    var buy = buys.Peek();
                    var sell = sells.Peek();

                    if (buy.Price < sell.Price)
                    {
                        break;
                    }

                    var amount = Math.Min(
                        amountRemaining[buy],
                        amountRemaining[sell]
                    );

                    var price = sell.Price + (buy.Price - sell.Price) / 2;
                    var result = new OfferResult(amount, price);

                    buy.Results.Add(result);
                    sell.Results.Add(result);

                    if (0 < amount)
                    {
                        amountRemaining[buy] -= amount;
                        amountRemaining[sell] -= amount;

                        succesfulTrades += 1;
                        moneyTraded += amount * price;
                        amountTraded += amount;
                    }

                    if (amountRemaining[buy] <= 0)
                    {
                        buys.Pop();
                    }

                    if (amountRemaining[sell] <= 0)
                    {
                        sells.Pop();
                    }
                }

                var avgPrice = moneyTraded / amountTraded;

                this.AddHistory(commodity, new MarketHistory
                {
                    Commodity = commodity,
                    SuccessfulTrades = succesfulTrades,
                    AmountToBuy = amountToBuy,
                    AmountToSell = amountToSell,
                    AmountTraded = amountTraded,
                    MoneyTraded = moneyTraded,
                    AveragePrice = avgPrice,
                    LowestSellingPrice = lowestSellingPrice,
                    HighestSellingPrice = highestSellingPrice,
                    LowestBuyingPrice = lowestBuyingPrice,
                    HighestBuyingPrice = highestBuyingPrice,
                });

            }

            this.offers.Clear();
        }

        private void AddHistory(string commodity, MarketHistory history)
        {
            if (!this.history.ContainsKey(commodity))
            {
                this.history[commodity] = new List<MarketHistory>();
            }

            var list = this.history[commodity];

            list.Insert(0, history);

            if (HISTORY_LIMIT < list.Count)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public IEnumerable<MarketHistory> GetHistory(string commodity)
        {
            return this.history.GetValueOrDefault(commodity, new List<MarketHistory>());
        }

        public IEnumerable<string> GetCommodities()
        {
            return this.history.Keys;
        }
    }
}
