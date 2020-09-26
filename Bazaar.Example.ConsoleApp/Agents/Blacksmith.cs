using Bazaar.Example.ConsoleApp.Behaviors;
using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Blacksmith : Agent
    {
        public Blacksmith(Town town) : base("blacksmith", town.Market)
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new BlacksmithBehavior(this, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class BlacksmithBehavior : AgentBehavior
    {
        private readonly EatBehavior eat;

        public BlacksmithBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var metal = this.Agent.Inventory.Get(Constants.Metal);
            var tools = this.Agent.Inventory.Get(Constants.Tools);
            var planks = this.Agent.Inventory.Get(Constants.Planks);

            if (tools < 4)
            {
                this.Agent.CostBeliefs.BeginUnit();

                var hasTools = 0 < tools;
                var eaten = this.eat.Eaten;

                var amount = new List<double> { metal, planks, eaten ? 2 : 1 }.Min();
                var factor = hasTools ? 2 : 1;

                this.Agent.Consume(Constants.Metal, amount);
                this.Agent.Consume(Constants.Planks, amount);
                this.Agent.Produce(Constants.Tools, factor * amount);

                if (hasTools && this.Random.NextDouble() < 0.1)
                {
                    this.Agent.Consume(Constants.Tools, 1);
                }

                this.Agent.CostBeliefs.EndUnit();
            }
            else
            {
                this.Agent.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Metal, 4);
            yield return this.Buy(Constants.Planks, 4);
            yield return this.Sell(Constants.Tools, 2);
        }
    }
}
