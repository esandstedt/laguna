using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Woodcutter : Agent
    {
        public Woodcutter(Market market) : base(market, "woodcutter")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new WoodcutterBehavior(this, eat));

            this.Inventory.Add(Constants.Bread, 2);
            this.Inventory.Add(Constants.Money, 30);
        }
    }

    public class WoodcutterBehavior : AgentBehavior
    {

        private readonly EatBehavior eat;

        public WoodcutterBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var wood = this.Agent.Inventory.Get(Constants.Wood);

            if (wood < 8)
            {
                var hasTools = 0 < this.Agent.Inventory.Get(Constants.Tools);
                var eaten = this.eat.Eaten;

                if (hasTools && eaten)
                {
                    this.Agent.Produce(Constants.Wood, 4);
                }
                else if (hasTools || eaten)
                {
                    this.Agent.Produce(Constants.Wood, 2);
                }
                else
                {
                    this.Agent.Produce(Constants.Wood, 1);
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
            yield return this.Sell(Constants.Wood);
        }
    }
}
