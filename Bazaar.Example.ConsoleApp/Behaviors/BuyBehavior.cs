using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class BuyBehavior : AgentBehavior
    {
        public List<Order> Orders { get; } = new List<Order>();

        public BuyBehavior(Agent agent) : base(agent) { }

        public override IEnumerable<Offer> GenerateOffers()
        {
            foreach (var order in this.Orders)
            {
                yield return this.Buy(order.Commodity, order.Amount, order.MaximumPrice);
            }
        }

        public class Order
        {
            public string Commodity { get; set; }
            public double MaximumPrice { get; set; }
            public double Amount { get; set; }
        }
    }
}
