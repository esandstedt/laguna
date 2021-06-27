using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class WorkerBehavior : AgentBehavior
    {
        public WorkerBehavior(Agent agent) : base(agent)
        {
        }

        public override void Perform()
        {
            var chance = this.Random.NextDouble();
            if (chance < 0.1)
            {
                this.Consume(Constants.Planks, 1);
            }
            else if (chance < 0.2)
            {
                this.Consume(Constants.Planks, 0.5);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Planks, 2);
        }
    }
}
