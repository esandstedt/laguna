using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
    public interface IAgentBehavior
    {
        void Perform();
        IEnumerable<Offer> GenerateOffers();
    }

    public class AgentBehavior : IAgentBehavior
    {
        protected Agent Agent { get; }

        protected Random Random = new Random();

        public AgentBehavior(Agent agent)
        {
            Agent = agent;
        }

        public virtual void Perform()
        {

        }

        public virtual IEnumerable<Offer> GenerateOffers()
        {
            return new List<Offer>();
        }

        protected Offer Buy(string commodity, double maxAmount)
        {
            return this.CreateBuyOffer(
                commodity,
                Math.Max(0, maxAmount - this.Agent.Inventory.Get(commodity))
            );
        }

        protected Offer CreateBuyOffer(string commodity, double amount)
        {
            var (minPrice, maxPrice) = this.Agent.PriceBeliefs.Get(commodity);
            var price = minPrice + this.Random.NextDouble() * (maxPrice - minPrice);

            return new Offer
            {
                Agent = this.Agent,
                Type = OfferType.Buy,
                Commodity = commodity,
                Amount = amount,
                Price = price
            };
        }

        protected Offer Sell(string commodity, double minAmount = 0)
        {
            return this.CreateSellOffer(
                commodity,
                Math.Max(0, this.Agent.Inventory.Get(commodity) - minAmount)
            );
        }

        protected Offer CreateSellOffer(string commodity, double amount)
        {
            var (minPrice, maxPrice) = this.Agent.PriceBeliefs.Get(commodity);
            var price = minPrice + this.Random.NextDouble() * (maxPrice - minPrice);

            return new Offer
            {
                Agent = this.Agent,
                Type = OfferType.Sell,
                Commodity = commodity,
                Amount = amount,
                Price = price
            };
        }
    }
}
