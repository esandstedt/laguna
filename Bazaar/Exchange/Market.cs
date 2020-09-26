using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bazaar.Exchange
{
    public class Market
    {

        private const int HISTORY_LIMIT = 20;
        private static OfferResult OFFERRESULT_FAILED = new OfferResult(false);

        private readonly Random random = new Random();
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
                        .Where(x => 0 < x.Amount)
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

                    var price = sell.Price + this.random.NextDouble() * (buy.Price - sell.Price);
                    var result = new OfferResult(true, price);

                    if (0 < amount)
                    {
                        amountRemaining[buy] -= amount;
                        amountRemaining[sell] -= amount;

                        sell.Principal.Inventory.Remove(commodity, amount);
                        buy.Principal.Inventory.Add(commodity, amount);

                        var money = amount * price;
                        buy.Principal.Inventory.Remove(Constants.Money, money);
                        sell.Principal.Inventory.Add(Constants.Money, money);

                        succesfulTrades += 1;
                        moneyTraded += money;
                        amountTraded += amount;

                        buy.Result = result;
                        sell.Result = result;
                    }

                    if (amountRemaining[buy] <= 0)
                    {
                        buy.Result = result;
                        buys.Pop();
                    }

                    if (amountRemaining[sell] <= 0)
                    {
                        sell.Result = result;
                        sells.Pop();
                    }
                }

                foreach (var buy in buys)
                {
                    buy.Result ??= OFFERRESULT_FAILED;
                }

                foreach (var sell in sells)
                {
                    sell.Result ??= OFFERRESULT_FAILED;
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

        public List<MarketHistory> GetHistory(string commodity)
        {
            return this.history.GetValueOrDefault(commodity, new List<MarketHistory>());
        }

        public List<string> GetCommodities()
        {
            return this.history.Keys.ToList();
        }
    }
}
