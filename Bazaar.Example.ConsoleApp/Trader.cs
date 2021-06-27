using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Trader : IAgent
    {
        public string Type => "trader";

        public Route Route { get; }

        public TraderPresence First { get; }
        public TraderPresence Second { get; }

        public Trader(Route route)
        {
            this.Route = route;

            this.First = new TraderPresence(route.First.Market);
            this.Second = new TraderPresence(route.Second.Market);
        }

        public void Perform()
        {
            // Transport commodities to other market
            foreach (var commodity in Constants.TradableCommodities)
            {
                var firstAmount = this.First.BuyInventory.Get(commodity);
                if (0 < firstAmount)
                {
                    this.Second.SellInventory.Add(commodity, firstAmount);
                    this.First.BuyInventory.Set(commodity, 0);

                    if (!this.Route.History.FirstExports.ContainsKey(commodity))
                    {
                        this.Route.History.FirstExports[commodity] = 0;
                    }

                    this.Route.History.FirstExports[commodity] += firstAmount;
                }

                var secondAmount = this.Second.BuyInventory.Get(commodity);
                if (0 < secondAmount)
                {
                    this.First.SellInventory.Add(commodity, secondAmount);
                    this.Second.BuyInventory.Set(commodity, 0);

                    if (!this.Route.History.SecondExports.ContainsKey(commodity))
                    {
                        this.Route.History.SecondExports[commodity] = 0;
                    }

                    this.Route.History.SecondExports[commodity] += secondAmount;
                }
            }
        }

        public void SubmitOffers()
        {
            var firstBuyOffers = this.GenerateBuyOffers(this.First, this.Second);
            this.First.SubmitOffers(firstBuyOffers);

            var secondBuyOffers = this.GenerateBuyOffers(this.Second, this.First);
            this.Second.SubmitOffers(secondBuyOffers);
        }

        private IEnumerable<Offer> GenerateBuyOffers(TraderPresence src, TraderPresence dest)
        {
            var money = src.BuyInventory.Get(Constants.Money);

            var trades = GenerateBestTrades(src, dest).Take(3).ToList();

            var ratio = 10 / trades.Sum(x => x.Item1);

            foreach (var trade in trades)
            {
                var diff = trade.Item1;
                var commodity = trade.Item2;
                var price = src.BuyPriceBeliefs.GetRandom(commodity);
                var amount = Math.Min(ratio * diff, money / price);

                yield return new Offer(
                    OfferType.Buy,
                    commodity,
                    price,
                    amount
                );

                money -= price * amount;
            }

            /*
            var trades = GenerateBestTrades(src, dest).Where(x => PROFIT_THRESHOLD < x.Item1).Take(3).ToList();
            if (trades.Any()) {
                var ratio = 10 / trades.Sum(x => x.Item1);

                foreach (var trade in trades) {
                    var diff = trade.Item1;
                    var commodity = trade.Item2;

                    yield return new Offer(
                        OfferType.Buy,
                        commodity,
                        src.BuyPriceBeliefs.GetRandom(commodity),
                        ratio * diff
                    );
                }
            }
             */
        }

        private IEnumerable<(double, string)> GenerateBestTrades(TraderPresence src, TraderPresence dest)
        {
            return Constants.TradableCommodities
                .Select(commodity =>
                {
                    var buyPrice = src.BuyPriceBeliefs.GetRandom(commodity);
                    var sellPrice = dest.SellPriceBeliefs.GetRandom(commodity);
                    var diff = sellPrice - buyPrice;
                    return (diff, commodity);
                })
                .Where(x => 0 < x.Item1)
                .OrderByDescending(x => x.Item1);
        }

        public void HandleOfferResults()
        {
            this.First.HandleOfferResults();
            this.Second.HandleOfferResults();

            // Split money evenly across the two presences.
            var money = this.First.BuyInventory.Get(Constants.Money) +
                this.First.SellInventory.Get(Constants.Money) +
                this.Second.BuyInventory.Get(Constants.Money) +
                this.Second.SellInventory.Get(Constants.Money);

            this.First.BuyInventory.Set(Constants.Money, money / 2);
            this.First.SellInventory.Set(Constants.Money, 0);
            this.Second.BuyInventory.Set(Constants.Money, money / 2);
            this.Second.SellInventory.Set(Constants.Money, 0);
        }
    }

    public class TraderPresence
    {
        public PriceBeliefs BuyPriceBeliefs { get; } = new PriceBeliefs();
        public Inventory BuyInventory = new Inventory();
        public PriceBeliefs SellPriceBeliefs { get; } = new PriceBeliefs();
        public Inventory SellInventory = new Inventory();
        
        private readonly IMarket market;
        private readonly List<Offer> offers = new List<Offer>();

        public TraderPresence(IMarket market)
        {
            this.market = market;

            this.BuyInventory.Add(Constants.Money, 100);
        }

        public void SubmitOffers(IEnumerable<Offer> buyOffers)
        {
            // Actual buy offers 
            foreach (var offer in buyOffers)
            {
                this.offers.Add(offer);
                this.market.AddOffer(offer);
            }

            foreach (var commodity in Constants.TradableCommodities)
            {
                // Prospective buy offers
                var buyPrice = this.BuyPriceBeliefs.GetRandom(commodity);
                var buyOffer = new Offer(OfferType.Buy, commodity, buyPrice, 0);
                this.offers.Add(buyOffer);
                this.market.AddOffer(buyOffer);

                // Prospective sell offers
                var sellPrice = this.SellPriceBeliefs.GetRandom(commodity);
                var sellOffer = new Offer(OfferType.Sell, commodity, sellPrice, 0);
                this.offers.Add(sellOffer);
                this.market.AddOffer(sellOffer);

                // Actual sell offers
                var amount = this.SellInventory.Get(commodity);
                if (0 < amount)
                {
                    var price = this.SellPriceBeliefs.GetRandom(commodity);
                    var offer = new Offer(OfferType.Sell, commodity, price, amount);
                    this.offers.Add(offer);
                    this.market.AddOffer(offer);
                }
            }
        }

        public void HandleOfferResults()
        {
            foreach (var offer in this.offers)
            {
                var amount = offer.Results.Sum(x => x.Amount);
                var money = offer.Results.Sum(x => x.Amount * x.Price);

                if (offer.Type == OfferType.Buy)
                {
                    this.BuyInventory.Add(offer.Commodity, amount);
                    this.BuyInventory.Remove(Constants.Money, money);

                    this.BuyPriceBeliefs.Update(offer);
                }
                else if (offer.Type == OfferType.Sell)
                {
                    this.SellInventory.Add(Constants.Money, money);
                    this.SellInventory.Remove(offer.Commodity, amount);

                    this.SellPriceBeliefs.Update(offer);
                }
            }

            this.offers.Clear();
        }
    }
}
