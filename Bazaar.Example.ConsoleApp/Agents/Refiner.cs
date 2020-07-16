using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Refiner : Agent
    {

        public Refiner() : base("refiner")
        {
            this.Buys("food", 2);
            this.Buys("ore", 8);
            this.Buys("tools", 2);
            this.Sells("metal");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 40);
        }

        protected override void PerformProduction()
        {
            var hasTooMuchMetal = 8 < this.Inventory.Get("metal");
            var hasFood = 0 < this.Inventory.Get("food");
            var hasTools = 0 < this.Inventory.Get("tools");


            if (!hasTooMuchMetal && hasFood && hasTools)
            {
                var amount = Math.Min(this.Inventory.Get("ore"), 4);
                this.Consume("ore", amount);
                this.Produce("metal", amount);
                
                this.Consume("food", 1);
                this.Consume("tools", 0.1);
            }
            else if (!hasTooMuchMetal && hasFood)
            {
                var amount = Math.Min(this.Inventory.Get("ore"), 4);
                this.Consume("ore", amount);
                this.Produce("metal", amount / 2);

                this.Consume("food", 1);
            }
            else
            {
                this.Consume("money", 2);
            }
        }
    }
}
