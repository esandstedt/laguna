using Bazaar.Example.ConsoleApp.Behaviors;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Sawyer : Agent
    {
        public Sawyer(Town town) : base("sawyer", town.Market)
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new SawyerBehavior(this, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }

        public class SawyerBehavior : AgentBehavior
        {

            private readonly EatBehavior eat;

            public SawyerBehavior(Agent agent, EatBehavior eat) : base(agent)
            {
                this.eat = eat;
            }

            public override void Perform()
            {
                var logs = this.Agent.Inventory.Get(Constants.Logs);
                var planks = this.Agent.Inventory.Get(Constants.Planks);
                var tools = this.Agent.Inventory.Get(Constants.Tools);

                if (planks < 32)
                {
                    this.Agent.CostBeliefs.BeginUnit();

                    var hasTools = 0 < tools;
                    var eaten = this.eat.Eaten;

                    var amount = Math.Min(logs, eaten ? 2 : 1);
                    var factor = hasTools ? 8 : 4;

                    this.Consume(Constants.Logs, amount);
                    this.Produce(Constants.Planks, factor * amount);

                    if (hasTools && this.Random.NextDouble() < 0.1)
                    {
                        this.Consume(Constants.Tools, 1);
                    }

                    this.Agent.CostBeliefs.EndUnit();
                }
                else
                {
                    //this.Consume(Constants.Money, 1);
                }
            }

            public override IEnumerable<Offer> GenerateOffers()
            {
                yield return this.Buy(Constants.Logs, 8);
                yield return this.Buy(Constants.Tools, 2);
                yield return this.Sell(Constants.Planks);
            }
        }
    }
}
