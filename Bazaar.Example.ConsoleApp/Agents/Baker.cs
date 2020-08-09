using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Baker : Agent
    {
        public Baker(Town town) : base(town.Market, "baker")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new BakerBehavior(this, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class BakerBehavior : AgentBehavior
    {
        private readonly EatBehavior eat;

        public BakerBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var flour = this.Agent.Inventory.Get(Constants.Flour);
            var bread = this.Agent.Inventory.Get(Constants.Bread);
            var tools = this.Agent.Inventory.Get(Constants.Tools);

            if (bread < 32)
            {
                var hasTools = 0 < tools;
                var eaten = this.eat.Eaten;

                var amount = Math.Min(flour, eaten ? 4 : 2);
                var factor = hasTools ? 4 : 2;

                this.Agent.Consume(Constants.Flour, amount);
                this.Agent.Produce(Constants.Bread, factor * amount);

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
            yield return this.Buy(Constants.Flour, 4);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Bread, 2);
        }
    }
}
