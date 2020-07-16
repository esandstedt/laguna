using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Refiner : BaseAgent
    {

        public Refiner(Market market) : base(market, "refiner")
        {
            this.Buys("food", 2);
            this.Buys("ore", 8);
            this.Buys("tools", 2);
            this.Sells("metal");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 50);
        }

        protected override void PerformProduction()
        {
            if (this.Metal < 8)
            {
                var hasTools = 0 < this.Food;
                var hasFood = 0 < this.Food;

                if (hasTools && hasFood)
                {
                    var amount = Math.Min(this.Ore, 4);
                    this.Consume("ore", amount);
                    this.Produce("metal", 0.75 * amount);
                }
                else if (hasFood)
                {
                    var amount = Math.Min(this.Ore, 4);
                    this.Consume("ore", amount);
                    this.Produce("metal", 0.5 * amount);
                }
                else
                {
                    var amount = Math.Min(this.Ore, 2);
                    this.Consume("ore", amount);
                    this.Produce("metal", 0.25 * amount);
                }
            }

            this.Eat();

            this.RestrictPriceBelief("ore");
        }
    }
}
