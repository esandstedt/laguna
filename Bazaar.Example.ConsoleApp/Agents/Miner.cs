﻿using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Miner : Agent
    {
        public Miner(Market market) : base(market, "miner")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new MinerBehavior(this, eat));

            this.Inventory.Add(Constants.Bread, 2);
            this.Inventory.Add(Constants.Money, 30);
        }
    }

    public class MinerBehavior : AgentBehavior
    {

        private readonly EatBehavior eat;

        public MinerBehavior(Agent agent, EatBehavior eat) : base(agent)
        {
            this.eat = eat;
        }

        public override void Perform()
        {
            var ore = this.Agent.Inventory.Get(Constants.Ore);

            if (ore < 8)
            {
                var hasTools = 0 < this.Agent.Inventory.Get(Constants.Tools);
                var eaten = this.eat.Eaten;

                if (hasTools && eaten)
                {
                    this.Agent.Produce(Constants.Ore, 4);
                }
                else if (hasTools)
                {
                    this.Agent.Produce(Constants.Ore, 2);
                }
                else if (eaten)
                {
                    this.Agent.Produce(Constants.Ore, 1);
                }
                else
                {
                    this.Agent.Produce(Constants.Ore, 0.5);
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
            yield return this.Sell(Constants.Ore);
        }
    }
}
