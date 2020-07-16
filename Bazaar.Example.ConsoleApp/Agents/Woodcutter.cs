using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Woodcutter : Agent
    {

        public Woodcutter() : base("woodcutter")
        {
            this.Buys("food", 2);
            this.Buys("tools", 2);
            this.Sells("wood");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 20);
        }

        protected override void PerformProduction()
        {
            var hasTooMuchWood = 4 < this.Inventory.Get("wood");
            var hasFood = 0 < this.Inventory.Get("food");
            var hasTools = 0 < this.Inventory.Get("tools");

            if (!hasTooMuchWood && hasFood && hasTools)
            {
                this.Produce("wood", 1);
                this.Consume("food", 1);
                this.Consume("tools", 0.1);
            }
            else if (!hasTooMuchWood && hasFood)
            {
                this.Produce("wood", 0.5);
                this.Consume("food", 1);
            }
            else if (hasTooMuchWood)
            {
                this.Consume("money", 2);
            }
            else
            {
                this.Produce("wood", 0.5);
                this.Consume("money", 2);
            }
        }
    }
}
