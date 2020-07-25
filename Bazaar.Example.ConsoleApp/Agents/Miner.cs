﻿using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Miner : Agent
    {
        public Miner(Market market, Area area) : base(market, "miner")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new ProduceBehavior(
                this,
                eat,
                new AreaProduceBehaviorOptions(area, Constants.Ore)
                {
                    EatFactor = 2,
                    ToolsFactor = 4,
                    ToolsBreakChance = 0.1
                }
            ));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }
}
