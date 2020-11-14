using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Trader : IAgent
    {
        private const double PROFIT_THRESHOLD = 1;

        public string Type => "trader";

        public Route Route { get; }

        private readonly TraderPresence first;
        private readonly TraderPresence second;

        public Trader(Route route)
        {
            this.Route = route;

            this.first = new TraderPresence(route.First.Market);
            this.second = new TraderPresence(route.Second.Market);
        }

        public void Perform()
        {
            // Transport commodities to other market
            foreach (var commodity in Constants.TradableCommodities)
            {
                var firstAmount = this.first.BuyInventory.Get(commodity);
                if (0 < firstAmount)
                {
                    this.second.SellInventory.Add(commodity, firstAmount);
                    this.first.BuyInventory.Set(commodity, 0);

                    if (!this.Route.History.FirstExports.ContainsKey(commodity))
                    {
                        this.Route.History.FirstExports[commodity] = 0;
                    }

                    this.Route.History.FirstExports[commodity] += firstAmount;
                }

                var secondAmount = this.second.BuyInventory.Get(commodity);
                if (0 < secondAmount)
                {
                    this.first.SellInventory.Add(commodity, secondAmount);
                    this.second.BuyInventory.Set(commodity, 0);

                    if (!this.Route.History.SecondExports.ContainsKey(commodity))
                    {
                        this.Route.History.SecondExports[commodity] = 0;
                    }

                    this.Route.History.SecondExports[commodity] += secondAmount;
                }
            }

            // Move money to buy inventory in both presences
            this.first.BuyInventory.Add(Constants.Money, this.first.SellInventory.Get(Constants.Money));
            this.first.SellInventory.Set(Constants.Money, 0);

            this.second.BuyInventory.Add(Constants.Money, this.second.SellInventory.Get(Constants.Money));
            this.second.SellInventory.Set(Constants.Money, 0);

            var money = this.first.BuyInventory.Get(Constants.Money) + this.second.BuyInventory.Get(Constants.Money);
        }

        public void SubmitOffers()
        {
            var firstBuyOffers = this.GenerateBuyOffers(this.first, this.second);
            this.first.SubmitOffers(firstBuyOffers);

            var secondBuyOffers = this.GenerateBuyOffers(this.second, this.first);
            this.second.SubmitOffers(secondBuyOffers);
        }

        private IEnumerable<Offer> GenerateBuyOffers(TraderPresence src, TraderPresence dest)
        {
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
            this.first.HandleOfferResults();
            this.second.HandleOfferResults();
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
                if (offer.Type == OfferType.Buy)
                {
                    this.BuyInventory.Add(offer.Commodity, offer.Amount);
                    this.BuyInventory.Remove(Constants.Money, offer.Price * offer.Amount);

                    this.BuyPriceBeliefs.Update(offer);
                }
                else if (offer.Type == OfferType.Sell)
                {
                    this.SellInventory.Remove(offer.Commodity, offer.Amount);
                    this.SellInventory.Add(Constants.Money, offer.Price * offer.Amount);

                    this.SellPriceBeliefs.Update(offer);
                }
            }

            this.offers.Clear();
        }
    }
}
