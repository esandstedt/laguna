using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class SellBehavior : AgentBehavior
    {
        public List<Order> Orders { get; } = new List<Order>();

        public SellBehavior(Agent agent) : base(agent) { }

        public override IEnumerable<Offer> GenerateOffers()
        {
            foreach (var order in this.Orders) {
                yield return this.Sell(order.Commodity, minAllowedPrice: order.MinimumPrice);
            }
        }

        public class Order
        {
            public string Commodity { get; set; }
            public double MinimumPrice { get; set; }
        }
    }

}
