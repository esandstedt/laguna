﻿using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Sawyer : Agent
    {
        public Sawyer(Market market) : base(market, "sawyer")
        {
            var eat = new EatBehavior(this);
            this.Behaviors.Add(eat);
            this.Behaviors.Add(new SawyerBehavior(this, eat));
            this.Behaviors.Add(new WorkerBehavior(this));

            this.Inventory.Add(Constants.Money, 100);
        }

        public class SawyerBehavior : AgentBehavior
        {

            private readonly EatBehavior eat;

            public SawyerBehavior(Agent agent, EatBehavior eat) : base(agent)
            {
                this.eat = eat;
            }

            public override void Perform()
            {
                var logs = this.Agent.Inventory.Get(Constants.Logs);
                var planks = this.Agent.Inventory.Get(Constants.Planks);
                var tools = this.Agent.Inventory.Get(Constants.Tools);

                if (planks < 32)
                {
                    var hasTools = 0 < tools;
                    var eaten = this.eat.Eaten;

                    var amount = Math.Min(logs, eaten ? 2 : 1);
                    var factor = hasTools ? 8 : 4;

                    this.Agent.Consume(Constants.Logs, amount);
                    this.Agent.Produce(Constants.Planks, factor * amount);

                    if (hasTools && this.Random.NextDouble() < 0.1)
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
                yield return this.Buy(Constants.Logs, 8);
                yield return this.Buy(Constants.Tools, 2);
                yield return this.Sell(Constants.Planks);
            }
        }
    }
}