using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Woodcutter : BaseAgent
    {

        public Woodcutter(Market market) : base(market, "woodcutter")
        {
            this.Buys("food", 2);
            this.Buys("tools", 2);
            this.Sells("wood");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 30);
        }

        protected override void PerformProduction()
        {
            if (this.Wood < 4)
            {
                var hasTools = 0 < this.Tools;
                var hasFood = 0 < this.Food;

                if (hasTools && hasFood)
                {
                    this.Produce("wood", 4);

                    if (this.Random.NextDouble() < 0.25)
                    {
                        this.Consume("tools", 1);
                    }
                }
                else if (hasFood)
                {
                    this.Produce("wood", 2);
                }
                else
                {
                    this.Produce("wood", 1);
                }
            }
            else
            {
                this.Consume("money", 2);
            }

            this.Eat();

            this.RestrictPriceBelief("wood");
        }
    }
}
