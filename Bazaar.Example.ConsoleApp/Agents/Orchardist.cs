﻿using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Orchardist : Agent
    {
        public Orchardist(Market market) : base(market, "orchardist")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new ProduceBehavior(
                this,
                eat,
                new ProduceBehaviorOptions
                {
                    Commodity = Constants.Apples,
                    BaseAmount = 0.1,
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