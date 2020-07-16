using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Blacksmith : Agent
    {
        public Blacksmith() : base("blacksmith")
        {
            this.Buys("food", 2);
            this.Buys("metal", 8);
            this.Sells("tools");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 50);
        }

        protected override void PerformProduction()
        {
            var hasTooManyTools = 8 < this.Inventory.Get("tools");
            var hasFood = 0 < this.Inventory.Get("food");

            var amount = Math.Min(this.Inventory.Get("metal"), 4);

            if (!hasTooManyTools && hasFood)
            {
                this.Consume("metal", amount);
                this.Produce("tools", amount);
                this.Consume("food", 1);
            }
            else
            {
                this.Consume("money", 2);
            }
        }
    }
}
