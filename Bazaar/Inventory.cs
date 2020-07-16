using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
    public class Inventory
    {

        private Dictionary<string, double> inventory { get; set; } = new Dictionary<string, double>();

        public double Get(string commodity)
        {
            this.EnsureKeyExists(commodity);

            return this.inventory[commodity];
        }

        public void Set(string commodity, double amount)
        {
            this.EnsureKeyExists(commodity);

            this.inventory[commodity] = amount;
        }

        public void Add(string commodity, double amount)
        {
            this.EnsureKeyExists(commodity);

            this.inventory[commodity] += amount;
        }

        public void Remove(string commodity, double amount)
        {
            this.EnsureKeyExists(commodity);

            this.inventory[commodity] -= amount;
        }

        private void EnsureKeyExists(string commodity)
        {
            if (!this.inventory.ContainsKey(commodity))
            {
                this.inventory[commodity] = 0;
            }
        }

    }
}
