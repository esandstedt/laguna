using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Market
{
    public class MarketImpl : IMarket
    {

        private readonly List<Offer> offers = new List<Offer>();
        public Dictionary<string, MarketHistory> History { get; set; } = new Dictionary<string, MarketHistory>();

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

                    var price = (sell.Price + buy.Price) / 2;
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

                this.History[commodity] = new MarketHistory
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
                };

            }

            this.offers.Clear();
        }
    }
}
