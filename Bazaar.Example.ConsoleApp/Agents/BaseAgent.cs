using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp.Agents
{
    public abstract class BaseAgent : Agent
    {

        protected Random Random = new Random();

        protected double Food => this.Inventory.Get("food");
        protected double Wood => this.Inventory.Get("wood");
        protected double Ore => this.Inventory.Get("ore");
        protected double Metal => this.Inventory.Get("metal");
        protected double Tools => this.Inventory.Get("tools");
        protected double Money => this.Inventory.Get("money");

        public BaseAgent(Market market, string type) : base(market, type)
        {

        }

        protected void Eat()
        {
            if (0 < this.Food)
            {
                this.Consume("food", 1);
            }
            else
            {
                this.Consume("money", 2);
            }
        }

        protected void RestrictPriceBelief(string commodity)
        {
            var belief = this.PriceBeliefs.Get(commodity);

            if (belief.Item1 < 1)
            {
                var factor = 1 / belief.Item1;
                this.PriceBeliefs.Set(commodity, factor * belief.Item1, factor * belief.Item2);
            }

            if (20 < belief.Item2)
            {
                var factor = 20 / belief.Item2;
                this.PriceBeliefs.Set(commodity, factor * belief.Item1, factor * belief.Item2);
            }
        }
    }
}
