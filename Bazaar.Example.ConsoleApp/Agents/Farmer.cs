using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Farmer : Agent
    {

        public Farmer() : base("farmer")
        {
            this.Buys("wood", 2);
            this.Buys("tools", 2);
            this.Sells("food");

            this.Inventory.Add("wood", 2);
            this.Inventory.Add("money", 20);
        }

        protected override void PerformProduction()
        {
            var hasTooMuchFood = 4 < this.Inventory.Get("food");
            var hasWood = 0 < this.Inventory.Get("wood");
            var hasTools = 0 < this.Inventory.Get("tools");

            if (!hasTooMuchFood & hasWood && hasTools)
            {
                this.Produce("food", 2);
                this.Consume("wood", 1);
                this.Consume("tools", 0.1);
            }
            else if (!hasTooMuchFood && hasWood)
            {
                this.Produce("food", 1);
                this.Consume("wood", 1);
            }
            else if (hasTooMuchFood)
            {
                this.Consume("money", 2);
            }
            else
            {
                this.Produce("food", 1);
                this.Consume("money", 2);
            }
        }
    }
}
