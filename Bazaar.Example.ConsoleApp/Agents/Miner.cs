using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Miner : BaseAgent
    {

        public Miner(Market market) : base(market, "miner")
        {
            this.Buys("food", 2);
            this.Buys("tools", 2);
            this.Sells("ore");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 30);
        }

        protected override void PerformProduction()
        {
            if (this.Ore < 4)
            {
                var hasTools = 0 < this.Tools;
                var hasFood = 0 < this.Food;

                if (hasTools && hasFood)
                {
                    this.Produce("ore", 4);

                    if (this.Random.NextDouble() < 0.25)
                    {
                        this.Consume("tools", 1);
                    }
                }
                else if (hasFood)
                {
                    this.Produce("ore", 1);
                }
                else
                {
                    this.Produce("ore", 0.5);
                }
            }
            else
            {
                this.Consume("money", 2);
            }

            this.Eat();

            this.RestrictPriceBelief("ore");
        }
    }
}
