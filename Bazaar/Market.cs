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

        public Market()
        {

        }

        public void Step()
        {
            var offers = new List<Offer>();

            foreach (var agent in Agents)
            {
                agent.Step();
                offers.AddRange(agent.GenerateOffers());
            }

            this.ResolveOffers(offers);
        }

        private void ResolveOffers(List<Offer> offers)
        {
            var groups = offers
                .Where(x => 1 < x.Amount)
                .GroupBy(x => x.Commodity);

            foreach (var group in groups)
            {
                var commodity = group.Key;

                var buyers = new Stack<Offer>(
                    group
                        .Where(x => x.Type == OfferType.Buy)
                        .OrderBy(x => x.Price)
                        .ToList()
                );

                var sellers = new Stack<Offer>(
                    group
                        .Where(x => x.Type == OfferType.Sell)
                        .OrderByDescending(x => x.Price)
                        .ToList()
                );

                { }

                int succesfulTrades = 0;
                double moneyTraded = 0;
                double amountTraded = 0;
                double amountToBuy = buyers.Sum(x => x.Amount);
                double amountToSell = sellers.Sum(x => x.Amount);

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
                    //var price = (buyer.Price + seller.Price) / 2;

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
                    }

                    if (buyer.Amount == 0)
                    {
                        buyer.Agent.UpdatePriceModel(OfferType.Buy, commodity, true, price);
                        buyers.Pop();
                    }

                    if (seller.Amount == 0)
                    {
                        seller.Agent.UpdatePriceModel(OfferType.Sell, commodity, true, price);
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

                this.EnsureMarketHistory(commodity);
                this.History[commodity].Add(new MarketHistory
                {
                    Commodity = commodity,
                    SuccessfulTrades = succesfulTrades,
                    AmountToBuy = amountToBuy,
                    AmountToSell = amountToSell,
                    AmountTraded = amountTraded,
                    MoneyTraded = moneyTraded,
                    AveragePrice = avgPrice,
                });
            }
        } 
        
        private void EnsureMarketHistory(string commodity)
        {
            if (!this.History.ContainsKey(commodity))
            {
                this.History[commodity] = new List<MarketHistory>();
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
    }
}
