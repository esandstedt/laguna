using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Blacksmith : BaseAgent
    {
        public Blacksmith(Market market) : base(market, "blacksmith")
        {
            this.Buys("food", 2);
            this.Buys("metal", 8);
            this.Sells("tools");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 50);
        }

        protected override void PerformProduction()
        {
            if (this.Tools < 8)
            {
                var hasFood = 0 < this.Food;

                if (hasFood)
                {
                    var amount = Math.Min(this.Inventory.Get("metal"), 4);
                    this.Consume("metal", amount);
                    this.Produce("tools", 0.5 * amount);
                }
                else
                {
                    var amount = Math.Min(this.Inventory.Get("metal"), 2);
                    this.Consume("metal", amount);
                    this.Produce("tools", 0.25 * amount);
                }
            }

            this.Eat();

            this.RestrictPriceBelief("tools");
        }
    }
}
