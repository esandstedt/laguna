using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class RecipeBuilder
    {
        private string key;
        private List<(string Commodity, double amount)> consumes;
        private string produces;
        private double producesAmount;

        public RecipeBuilder(string key)
        {
            this.key = key;
            this.consumes = new List<(string Commodity, double amount)>();
        }

        public RecipeBuilder Consumes(string commodity, double amount)
        {
            this.consumes.Add((commodity, amount));
            return this;
        }

        public RecipeBuilder Produces(string commodity, double amount)
        {
            this.produces = commodity;
            this.producesAmount = amount;
            return this;
        }

        public Recipe Build()
        {
            return new Recipe
            {
                Key = this.key,
                Consumes = this.consumes
                    .Select(x => (x.Commodity, x.amount / this.producesAmount))
                    .ToList(),
                Produces = this.produces
            };
        }
    }
}
