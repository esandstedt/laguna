using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Fisherman : Agent
    {
        public Fisherman(Market market) : base(market, "fisherman")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new FishermanBehavior(this, eat));

            this.Inventory.Add(Constants.Bread, 2);
            this.Inventory.Add(Constants.Money, 30);
        }
    }

    public class FishermanBehavior : AgentBehavior
    {
        private readonly EatBehavior eat;

        public FishermanBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var fish = this.Agent.Inventory.Get(Constants.Fish);

            if (fish < 16)
            {
                var hasTools = 0 < this.Agent.Inventory.Get(Constants.Tools);
                var eaten = this.eat.Eaten;

                if (hasTools && eaten)
                {
                    this.Agent.Produce(Constants.Fish, 8);
                }
                else if (hasTools || eaten)
                {
                    this.Agent.Produce(Constants.Fish, 4);
                }
                else
                {
                    this.Agent.Produce(Constants.Fish, 1);
                }

                if (hasTools && this.Random.NextDouble() < 0.25)
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
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Fish, 2);
        }


    }
}
