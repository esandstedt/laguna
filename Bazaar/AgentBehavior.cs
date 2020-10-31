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

        protected Offer Buy(string commodity, double maxAmount, double maxAllowedPrice = double.MaxValue)
        {
            var current = this.Agent.Inventory.Get(commodity);

            if (current < maxAmount)
            {
                var (minPrice, maxPrice) = this.Agent.PriceBeliefs.Get(commodity);

                minPrice = Math.Min(minPrice, maxAllowedPrice);
                maxPrice = Math.Min(maxPrice, maxAllowedPrice);

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

        protected Offer Sell(string commodity, double minAmount = 0, double minAllowedPrice = 0)
        {
            var current = this.Agent.Inventory.Get(commodity);

            if (minAmount < current)
            {
                var (minPrice, maxPrice) = this.Agent.PriceBeliefs.Get(commodity);

                minPrice = Math.Max(minPrice, minAllowedPrice);
                maxPrice = Math.Max(maxPrice, minAllowedPrice);

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

        protected void Consume(string commodity, double amount)
        {
            this.Agent.Inventory.Remove(commodity, amount);
            this.Agent.CostBeliefs.Consume(commodity, amount);
        }

        protected void Produce(string commodity, double amount)
        {
            this.Agent.Inventory.Add(commodity, amount);
            this.Agent.CostBeliefs.Produce(commodity, amount);
        }
    }
}
