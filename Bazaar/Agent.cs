using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar
{
    public abstract class Agent : IOfferPrincipal
    {

        private static readonly double MINIMUM_PRICE = 1;
        private static readonly double MAXIMUM_PRICE = 100;

        public string Type { get; }
        public List<AgentBehavior> Behaviors { get; }
        public Market Market { get; }
        public Inventory Inventory { get; } 
        public PriceBeliefs PriceBeliefs { get; }
        public CostBeliefs CostBeliefs { get; }

        public Agent(Market market, string type)
        {
            this.Market = market;
            this.Type = type;
            this.Behaviors = new List<AgentBehavior>();
            this.Inventory = new Inventory();
            this.PriceBeliefs = new PriceBeliefs(MINIMUM_PRICE, MAXIMUM_PRICE);
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs, MINIMUM_PRICE);

            this.InitializePriceBeliefs(market);
        }

        private void InitializePriceBeliefs(Market market)
        {
            foreach (var commodity in market.GetCommodities())
            {
                IList<MarketHistory> history = market.GetHistory(commodity);
                if (history.Any())
                {
                    var list = history.Take(10).ToList();

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

        public virtual void Step()
        {
            this.CostBeliefs.Begin();

            foreach (var behavior in this.Behaviors)
            {
                behavior.Perform();
            }

            this.CostBeliefs.End();

            this.CreateOffers();
        }

        private void CreateOffers()
        {
            var money = this.Inventory.Get(Constants.Money);

            var offers = this.Behaviors
                .SelectMany(x => x.GenerateOffers())
                .Where(x => x != null)
                .ToList();

            var buyOffers = offers.Where(x => x.Type == OfferType.Buy)
                .SelectMany(x => x.Split(1))
                .OrderBy(x => Guid.NewGuid());

            var sellOffers = offers.Where(x => x.Type == OfferType.Sell);

            foreach (var offer in buyOffers)
            {
                money -= offer.Price * offer.Amount;

                if (money < 0)
                {
                    break;
                }

                this.Market.AddOffer(offer);
            }

            foreach (var offer in sellOffers)
            {
                this.Market.AddOffer(offer);
            }
        }

        public void Consume(string commodity, double amount)
        {
            this.Inventory.Remove(commodity, amount);
            this.CostBeliefs.Consume(commodity, amount);
        }

        public void Produce(string commodity, double amount)
        {
            this.Inventory.Add(commodity, amount);
            this.CostBeliefs.Produce(commodity, amount);
        }

        void IOfferPrincipal.AddInventory(string commodity, double amount)
        {
            this.Inventory.Add(commodity, amount);
        }

        void IOfferPrincipal.RemoveInventory(string commodity, double amount)
        {
            this.Inventory.Remove(commodity, amount);
        }

        void IOfferPrincipal.UpdatePriceModel(OfferType type, string commodity, bool success, double price)
        {
            var (minPrice, maxPrice) = this.PriceBeliefs.Get(commodity);
            var money = this.Inventory.Get(Constants.Money);

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
    }
}
