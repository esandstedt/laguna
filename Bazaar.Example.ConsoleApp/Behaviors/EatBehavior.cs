using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Behaviors
{
    public class EatBehavior : AgentBehavior
    {

        public EatBehavior(Agent agent) : base(agent) 
        {
        }

        public bool Eaten { get; private set; }

        public override void Perform()
        {
            var bread = this.Agent.Inventory.Get(Constants.Bread);
            var fish = this.Agent.Inventory.Get(Constants.Fish);

            var food = bread + fish;


            if (1 < food)
            {
                this.Agent.Consume(Constants.Bread, Math.Min(bread, 0.75));
                this.Agent.Consume(Constants.Fish, Math.Min(fish, 0.75));
                this.Eaten = true;
            }
            else
            {
                this.Agent.Consume(Constants.Money, 2);
                this.Eaten = false;
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Bread, 2);
            yield return this.Buy(Constants.Fish, 2);
        }
    }
}
