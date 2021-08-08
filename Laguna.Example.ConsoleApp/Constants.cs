using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    class Constants
    {
        public const string Money = Laguna.Agent.Constants.Money;

        public const string UnskilledWork = "unskilled-work";
        public const string Grain = "grain";
        public const string Fruit = "fruit";
        public const string Vegetables = "vegetables";
        public const string Fish = "fish";
        public const string Meat = "meat";
        public const string Wood = "wood";
        public const string Timber = "timber";
        public const string Bread = "bread";

        public static readonly Dictionary<string, Commodity> Commodities = new List<Commodity>
        {
            new Commodity(UnskilledWork, "UnskilledWork", 1.0),
            new Commodity(Grain, "Grain", 0.05),
            new Commodity(Fruit, "Fruit", 0.1),
            new Commodity(Vegetables, "Vegetables", 0.1),
            new Commodity(Fish, "Fish", 0.15),
            new Commodity(Meat, "Meat", 0.15),
            new Commodity(Wood, "Wood", 0.02),
            new Commodity(Timber, "Timber", 0.02),
            new Commodity(Bread, "Bread", 0.1),
        }
            .ToDictionary(x => x.Key, x => x);


        public static readonly Dictionary<string, Recipe> Recipes = new List<Recipe>
        {
            new RecipeBuilder(Grain)
                .Consumes(UnskilledWork, 1)
                .Produces(Grain, 8)
                .Build(),
            new RecipeBuilder(Fruit)
                .Consumes(UnskilledWork, 1)
                .Produces(Fruit, 8)
                .Build(),
            new RecipeBuilder(Vegetables)
                .Consumes(UnskilledWork, 1)
                .Produces(Vegetables, 8)
                .Build(),
            new RecipeBuilder(Meat)
                .Consumes(UnskilledWork, 1)
                .Produces(Meat, 8)
                .Build(),
            new RecipeBuilder(Fish)
                .Consumes(UnskilledWork, 1)
                .Produces(Fish, 8)
                .Build(),
            new RecipeBuilder(Wood)
                .Consumes(UnskilledWork, 1)
                .Produces(Wood, 8)
                .Build(),
            new RecipeBuilder(Timber)
                .Consumes(UnskilledWork, 1)
                .Consumes(Wood, 1)
                .Produces(Timber, 4)
                .Build(),
            new RecipeBuilder(Bread)
                .Consumes(UnskilledWork, 1)
                .Consumes(Grain, 1)
                .Produces(Bread, 4)
                .Build()
        }
            .ToDictionary(x => x.Key, x => x);

    }
}
