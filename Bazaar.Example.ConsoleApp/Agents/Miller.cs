﻿using Bazaar.Example.ConsoleApp.Behaviors;
using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Miller : Agent
    {
        public Miller(Town town) : base("miller", town.Market)
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new MillerBehavior(this, town.Area, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }
    }

    public class MillerBehavior : AgentBehavior
    {

        private readonly EatBehavior eat;
        private readonly double ratio;

        public MillerBehavior(Agent agent, Area area, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
            this.ratio = area.Production[Constants.Flour];
        }

        public override void Perform()
        {
            var grain = this.Agent.Inventory.Get(Constants.Grain);
            var flour = this.Agent.Inventory.Get(Constants.Flour);
            var tools = this.Agent.Inventory.Get(Constants.Tools);

            if (flour < 32)
            {
                this.Agent.CostBeliefs.BeginUnit();

                var hasTools = 0 < tools;
                var eaten = this.eat.Eaten;

                var amount = Math.Min(grain, eaten ? 2 : 1);
                var factor = hasTools ? 0.75 : 0.5;

                this.Consume(Constants.Grain, amount);
                this.Produce(Constants.Flour, this.ratio * factor * amount);

                if (hasTools && this.Random.NextDouble() < 0.1)
                {
                    this.Consume(Constants.Tools, 1);
                }

                this.Agent.CostBeliefs.EndUnit();
            }
            else
            {
                this.Consume(Constants.Money, 1);
            }
        }

        public override IEnumerable<Offer> GenerateOffers()
        {
            yield return this.Buy(Constants.Grain, 8);
            yield return this.Buy(Constants.Tools, 2);
            yield return this.Sell(Constants.Flour);
        }
    }
}
