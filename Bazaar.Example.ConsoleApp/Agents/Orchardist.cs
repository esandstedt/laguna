using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Orchardist : Agent
    {
        public Orchardist(Town town) : base("orchardist", town.Market)
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);

            this.Behaviors.Add(new ProduceBehavior(
                this,
                town,
                eat,
                new AreaProduceBehaviorOptions(this, town, Constants.Apples)
                {
                    BaseAmount = 1,
                    EatFactor = 2,
                    ToolsFactor = 2,
                    ToolsBreakChance = 0.1
                }
            ));
            this.Behaviors.Add(new ProduceBehavior(
                this,
                town,
                eat,
                new AreaProduceBehaviorOptions(this, town, Constants.Oranges)
                {
                    BaseAmount = 1,
                    EatFactor = 2,
                    ToolsFactor = 2,
                    ToolsBreakChance = 0.1
                }
            ));

            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);

        }
    }
}
