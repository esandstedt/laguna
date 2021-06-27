using Bazaar.Example.ConsoleApp.Behaviors;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Refiner : Agent
    {
        public Refiner(Town town) : base("refiner", town.Market)
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new RefinerBehavior(this, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class RefinerBehavior : AgentBehavior
    {

        private readonly EatBehavior eat;

        public RefinerBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var ore = this.Agent.Inventory.Get(Constants.Ore);
            var metal = this.Agent.Inventory.Get(Constants.Metal);
            var tools = this.Agent.Inventory.Get(Constants.Tools);

            if (metal < 8)
            {
                this.Agent.CostBeliefs.BeginUnit();

                var hasTools = 0 < tools;
                var eaten = this.eat.Eaten;

                var amount = Math.Min(ore, eaten ? 4 : 2);
                var factor = hasTools ? 0.5 : 0.25;

                this.Consume(Constants.Ore, amount);
                this.Produce(Constants.Metal, factor * amount);

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
            yield return this.Buy(Constants.Ore, 8);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Metal);
        }
    }
}
