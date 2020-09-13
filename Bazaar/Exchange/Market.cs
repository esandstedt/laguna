using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bazaar.Exchange
{
    public class Market
    {

        private const int HISTORY_LIMIT = 20;

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

                    var amount = Math.Min(buy.Amount, sell.Amount);
                    var price = sell.Price + this.random.NextDouble() * (buy.Price - sell.Price);

                    if (0 < amount)
                    {

                        buy.Amount -= amount;
                        sell.Amount -= amount;

                        sell.Principal.RemoveInventory(commodity, amount);
                        buy.Principal.AddInventory(commodity, amount);

                        var money = amount * price;
                        buy.Principal.RemoveInventory(Constants.Money, money);
                        sell.Principal.AddInventory(Constants.Money, money);

                        succesfulTrades += 1;
                        moneyTraded += money;
                        amountTraded += amount;

                        buy.Principal.UpdatePriceModel(OfferType.Buy, commodity, true, price);
                        sell.Principal.UpdatePriceModel(OfferType.Sell, commodity, true, price);
                    }

                    if (buy.Amount == 0)
                    {
                        buy.Principal.UpdatePriceModel(OfferType.Buy, commodity, true, price);
                        buys.Pop();
                    }

                    if (sell.Amount == 0)
                    {
                        sell.Principal.UpdatePriceModel(OfferType.Sell, commodity, true, price);
                        sells.Pop();
                    }
                }

                foreach (var buy in buys)
                {
                    buy.Principal.UpdatePriceModel(OfferType.Buy, commodity, false);
                }

                foreach (var sell in sells)
                {
                    sell.Principal.UpdatePriceModel(OfferType.Sell, commodity, false);
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

                this.offers.Clear();
            }
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
