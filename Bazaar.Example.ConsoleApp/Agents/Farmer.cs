using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Farmer : BaseAgent
    {
        public Farmer(Market market) : base(market, "farmer")
        {
            this.Buys("wood", 2);
            this.Buys("tools", 2);
            this.Sells("food");

            this.Inventory.Add("wood", 2);
            this.Inventory.Add("money", 30);
        }

        protected override void PerformProduction()
        {
            if (this.Food < 8)
            {
                var hasTools = 0 < this.Tools;
                var hasWood = 0 < this.Wood;

                if (hasTools && hasWood)
                {
                    this.Produce("food", 4);
                    this.Consume("wood", 1);

                    if (this.Random.NextDouble() < 0.25)
                    {
                        this.Consume("tools", 1);
                    }
                }
                else if (hasWood)
                {
                    this.Produce("food", 3);
                    this.Consume("wood", 1);
                }
                else
                {
                    this.Produce("food", 2);
                }
            }
            else
            {
                this.Consume("money", 2);
            }

            this.Eat();

            this.RestrictPriceBelief("food");
        }
    }
}
