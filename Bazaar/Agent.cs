using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar
{
    public abstract class Agent 
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
            foreach (var commodity in market.History.Keys)
            {
                IList<MarketHistory> history = market.History.GetValueOrDefault(commodity);
                if (history != null)
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
        }

        public IEnumerable<Offer> GenerateOffers()
        {
            var money = this.Inventory.Get("money");

            var offers = this.Behaviors
                .SelectMany(x => x.GenerateOffers())
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

                yield return offer;
            }

            foreach (var offer in sellOffers)
            {
                yield return offer;
            }
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

    }
}
