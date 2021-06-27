using Bazaar.Example.ConsoleApp.Behaviors;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Farmer : Agent
    {
        public Farmer(Town town) : base("farmer", town.Market)
        {
            this.Behaviors.Add(new EatBehavior(this));
            this.Behaviors.Add(new FarmerBehavior(this, town));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Planks, 2);
            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class FarmerBehavior : AgentBehavior
    {
        private readonly Town town;

        public FarmerBehavior(Agent agent, Town town) : base(agent) 
        {
            this.town = town;
        }

        public override void Perform()
        {
            var grain = this.Agent.Inventory.Get(Constants.Grain);
            var tools = this.Agent.Inventory.Get(Constants.Tools);
            var planks = this.Agent.Inventory.Get(Constants.Planks);

            if (grain < 4)
            {
                var hasTools = 0 < tools;
                var hasPlanks = 0 < planks;

                var ratio = this.town.GetRatio(Constants.Grain);

                if (hasTools && hasPlanks)
                {
                    this.Agent.CostBeliefs.BeginUnit();

                    this.Produce(Constants.Grain, ratio * 3);
                    this.Consume(Constants.Planks, 0.25);

                    if (this.Random.NextDouble() < 0.1)
                    {
                        this.Consume(Constants.Tools, 1);
                    }

                    this.Agent.CostBeliefs.EndUnit();
                }
                else if (hasPlanks)
                {
                    this.Agent.CostBeliefs.BeginUnit();

                    this.Produce(Constants.Grain, ratio * 1);
                    this.Consume(Constants.Planks, 0.5);

                    this.Agent.CostBeliefs.EndUnit();
                }
                else
                {
                    //this.Consume(Constants.Money, 1);
                }
            }
            else
            {
                //this.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Planks, 2);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Grain);
        }
    }
}
