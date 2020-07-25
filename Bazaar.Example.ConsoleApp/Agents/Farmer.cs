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
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Planks, 2);
            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class FarmerBehavior : AgentBehavior
    {

        public FarmerBehavior(Agent agent) : base(agent) { }

        public override void Perform()
        {
            var grain = this.Agent.Inventory.Get(Constants.Grain);
            var tools = this.Agent.Inventory.Get(Constants.Tools);
            var planks = this.Agent.Inventory.Get(Constants.Planks);

            if (grain < 4)
            {
                var hasTools = 0 < tools;
                var hasPlanks = 0 < planks;

                if (hasTools && hasPlanks)
                {
                    this.Agent.Produce(Constants.Grain, 1.5);
                    this.Agent.Consume(Constants.Planks, 0.25);

                    if (this.Random.NextDouble() < 0.1)
                    {
                        this.Agent.Consume(Constants.Tools, 1);
                    }
                }
                else if (hasPlanks)
                {
                    this.Agent.Produce(Constants.Grain, 0.5);
                    this.Agent.Consume(Constants.Planks, 0.5);
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
            yield return this.Buy(Constants.Planks, 2);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Grain);
        }
    }
}
