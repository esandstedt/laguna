﻿using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Fisherman : Agent
    {
        public Fisherman(Town town) : base(town.Market, "fisherman")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new ProduceBehavior(
                this,
                eat,
                new AreaProduceBehaviorOptions(town.Area, Constants.Fish)
                {
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
