using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
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
            var current = this.Agent.Inventory.Get(commodity);

            if (current < maxAmount)
            {
                var (minPrice, maxPrice) = this.Agent.PriceBeliefs.Get(commodity);
                var price = minPrice + this.Random.NextDouble() * (maxPrice - minPrice);

                return new Offer(
                    this.Agent,
                    OfferType.Buy,
                    commodity,
                    price,
                    maxAmount - current
                );
            }

            return null;
        }

        protected Offer Sell(string commodity, double minAmount = 0)
        {
            var current = this.Agent.Inventory.Get(commodity);

            if (minAmount < current)
            {
                var (minPrice, maxPrice) = this.Agent.PriceBeliefs.Get(commodity);
                var price = minPrice + this.Random.NextDouble() * (maxPrice - minPrice);

                return new Offer(
                    this.Agent,
                    OfferType.Sell,
                    commodity,
                    price,
                    current - minAmount
                );
            }

            return null;
        }
    }
}
