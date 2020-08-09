﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bazaar
{
    public class Market
    {

        private Random random = new Random();

        public List<Agent> Agents { get; set; } = new List<Agent>();

        public Dictionary<string, List<MarketHistory>> History { get; set; } = new Dictionary<string, List<MarketHistory>>();

        public void Step()
        {
            var offers = this.Agents
                .SelectMany(x => x.GenerateOffers())
                .ToList();

            foreach (var group in offers.GroupBy(x => x.Commodity))
            {
                var commodity = group.Key;

                var buyers = new Stack<Offer>(
                    group
                        .Where(x => x.Type == OfferType.Buy)
                        .Where(x => 0 < x.Amount)
                        .OrderBy(x => x.Price)
                        .ToList()
                );

                var sellers = new Stack<Offer>(
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
                double amountToBuy = buyers.Sum(x => x.Amount);
                double amountToSell = sellers.Sum(x => x.Amount);
                double lowestSellingPrice = sellers.Any() ? sellers.Min(x => x.Price) : 1;
                double highestSellingPrice = sellers.Any() ? sellers.Max(x => x.Price) : 1;
                double lowestBuyingPrice = buyers.Any() ? buyers.Min(x => x.Price) : 1;
                double highestBuyingPrice = buyers.Any() ? buyers.Max(x => x.Price) : 1;

                while (buyers.Count != 0 && sellers.Count != 0)
                {
                    var buyer = buyers.Peek();
                    var seller = sellers.Peek();

                    if (buyer.Price < seller.Price)
                    {
                        break;
                    }

                    var amount = Math.Min(buyer.Amount, seller.Amount);
                    var price = seller.Price + this.random.NextDouble() * (buyer.Price - seller.Price);

                    if (0 < amount)
                    {
                        buyer.Amount -= amount;
                        seller.Amount -= amount;

                        seller.Agent.Inventory.Remove(commodity, amount);
                        buyer.Agent.Inventory.Add(commodity, amount);

                        var money = amount * price;
                        buyer.Agent.Inventory.Remove("money", money);
                        seller.Agent.Inventory.Add("money", money);

                        succesfulTrades += 1;
                        moneyTraded += money;
                        amountTraded += amount;

                        buyer.Agent.UpdatePriceModel(OfferType.Buy, commodity, true, price);
                        seller.Agent.UpdatePriceModel(OfferType.Sell, commodity, true, price);
                    }

                    if (buyer.Amount == 0)
                    {
                        buyers.Pop();
                    }

                    if (seller.Amount == 0)
                    {
                        sellers.Pop();
                    }
                }

                foreach (var buyer in buyers)
                {
                    buyer.Agent.UpdatePriceModel(OfferType.Buy, commodity, false);
                }

                foreach (var seller in sellers)
                {
                    seller.Agent.UpdatePriceModel(OfferType.Sell, commodity, false);
                }

                var avgPrice = moneyTraded / amountTraded;

                this.AddMarketHistory(commodity, new MarketHistory
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

                { }
            }
        } 

        private void AddMarketHistory(string commodity, MarketHistory history)
        {
            if (!this.History.ContainsKey(commodity))
            {
                this.History[commodity] = new List<MarketHistory>();
            }


            var list = this.History[commodity];

            list.Insert(0, history);

            if (20 < list.Count)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }

    public class MarketHistory
    {

        public string Commodity { get; set; }
        public int SuccessfulTrades { get; set; }
        public double MoneyTraded { get; set; }
        public double AmountTraded { get; set; }
        public double AmountToBuy { get; set; }
        public double AmountToSell { get; set; }
        public double AveragePrice { get; set; }
        public double LowestSellingPrice { get; set; }
        public double HighestSellingPrice { get; set; }
        public double LowestBuyingPrice { get; set; }
        public double HighestBuyingPrice { get; set; }
    }
}
