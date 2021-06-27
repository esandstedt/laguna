using Bazaar.Example.ConsoleApp.Behaviors;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Baker : Agent
    {
        public Baker(Town town) : base("baker", town.Market)
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new BakerBehavior(this, town, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class BakerBehavior : AgentBehavior
    {
        private readonly Town town;
        private readonly EatBehavior eat;

        public BakerBehavior(Agent agent, Town town, EatBehavior eat) : base(agent)
        {
            this.town = town;
            this.eat = eat;
        }

        public override void Perform()
        {
            var ratio = this.town.GetRatio(Constants.Bread);

            var flour = this.Agent.Inventory.Get(Constants.Flour);
            var bread = this.Agent.Inventory.Get(Constants.Bread);
            var tools = this.Agent.Inventory.Get(Constants.Tools);

            if (bread < 32)
            {
                this.Agent.CostBeliefs.BeginUnit();

                var hasTools = 0 < tools;
                var eaten = this.eat.Eaten;

                var amount = Math.Min(flour, eaten ? 4 : 2);
                var factor = hasTools ? 4 : 2;

                this.Consume(Constants.Flour, amount);
                this.Produce(Constants.Bread, ratio * factor * amount);

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
            yield return this.Buy(Constants.Flour, 4);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Bread, 2);
        }
    }
}
