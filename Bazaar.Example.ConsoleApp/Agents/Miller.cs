using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Miller : Agent
    {
        public Miller(Market market) : base(market, "miller")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new MillerBehavior(this, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class MillerBehavior : AgentBehavior
    {

        private readonly EatBehavior eat;

        public MillerBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var grain = this.Agent.Inventory.Get(Constants.Grain);
            var flour = this.Agent.Inventory.Get(Constants.Flour);
            var tools = this.Agent.Inventory.Get(Constants.Tools);

            if (flour < 32)
            {
                var hasTools = 0 < tools;
                var eaten = this.eat.Eaten;

                var amount = Math.Min(grain, eaten ? 4 : 2);
                var factor = hasTools ? 0.75 : 0.5;

                this.Agent.Consume(Constants.Grain, amount);
                this.Agent.Produce(Constants.Flour, factor * amount);

                if (hasTools && this.Random.NextDouble() < 0.1)
                {
                    this.Agent.Consume(Constants.Tools, 1);
                }
            }
            else
            {
                this.Agent.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Grain, 8);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Flour);
        }
    }
}
