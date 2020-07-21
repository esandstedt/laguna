using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Farmer : Agent
    {
        public Farmer(Market market) : base(market, "farmer")
        {
            this.Behaviors.Add(new EatBehavior(this));
            this.Behaviors.Add(new FarmerBehavior(this));

            this.Inventory.Add(Constants.Wood, 2);
            this.Inventory.Add(Constants.Money, 30);
        }
    }

    public class FarmerBehavior : AgentBehavior
    {

        public FarmerBehavior(Agent agent) : base(agent) { }

        public override void Perform()
        {
            var wheat = this.Agent.Inventory.Get(Constants.Wheat);
            var tools = this.Agent.Inventory.Get(Constants.Tools);
            var wood = this.Agent.Inventory.Get(Constants.Wood);

            if (wheat < 4)
            {
                var hasTools = 0 < tools;
                var hasWood = 0 < wood;

                if (hasTools && hasWood)
                {
                    this.Agent.Produce(Constants.Wheat, 0.5);
                    this.Agent.Consume(Constants.Wood, 0.25);

                    if (this.Random.NextDouble() < 0.1)
                    {
                        this.Agent.Consume(Constants.Tools, 1);
                    }
                }
                else if (hasWood)
                {
                    this.Agent.Produce(Constants.Wheat, 0.25);
                    this.Agent.Consume(Constants.Wood, 0.5);
                }
                else
                {
                    this.Agent.Consume(Constants.Money, 1);
                }
            }
            else
            {
                this.Agent.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Wood, 2);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Wheat);
        }
    }
}
