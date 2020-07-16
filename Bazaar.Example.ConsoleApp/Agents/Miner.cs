using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public class Miner : Agent
    {

        public Miner() : base("miner")
        {
            this.Buys("food", 2);
            this.Buys("tools", 2);
            this.Sells("ore");

            this.Inventory.Add("food", 2);
            this.Inventory.Add("money", 20);
        }

        protected override void PerformProduction()
        {
            var hasTooMuchOre = 4 < this.Inventory.Get("ore");
            var hasFood = 0 < this.Inventory.Get("food");
            var hasTools = 0 < this.Inventory.Get("tools");

            if (!hasTooMuchOre && hasFood && hasTools)
            {
                this.Produce("ore", 1);
                this.Consume("food", 1);
                this.Consume("tools", 0.1);
            }
            else if (!hasTooMuchOre && hasFood)
            {
                this.Produce("ore", 0.5);
                this.Consume("food", 1);
            }
            else if (hasTooMuchOre)
            {
                this.Consume("money", 2);
            }
            else
            {
                this.Produce("ore", 0.5);
                this.Consume("money", 2);
            }
        }
    }
}
