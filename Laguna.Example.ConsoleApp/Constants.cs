using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    class Constants
    {
        private const double SPOIL_LOW = 0.01;
        private const double SPOIL_MED = 0.05;
        private const double SPOIL_HIGH = 0.15;
        private const double SPOIL_ALL = 1.00;

        public const string Money = Laguna.Agent.Constants.Money;

        public const string UnskilledWork = "unskilled-work";
        public const string Grain = "grain";
        public const string Fruit = "fruit";
        public const string Vegetables = "vegetables";
        public const string Hops = "hops";
        public const string Fish = "fish";
        public const string Meat = "meat";
        public const string Bread = "bread";
        public const string Wine = "wine";
        public const string Beer = "beer";

        public const string Wood = "wood";
        public const string Timber = "timber";
        public const string Barrel = "barrel";
        public const string Furniture = "furniture";

        public const string Cotton = "cotton";
        public const string Fabric = "fabric";
        public const string Clothes = "clothes";


        public static readonly Dictionary<string, Commodity> Commodities = new List<Commodity>
        {
            new Commodity(UnskilledWork, "UnskilledWork", SPOIL_ALL),
            new Commodity(Grain, "Grain", SPOIL_LOW),
            new Commodity(Fruit, "Fruit", SPOIL_MED),
            new Commodity(Vegetables, "Vegetables", SPOIL_MED),
            new Commodity(Hops, "Hops", SPOIL_LOW),
            new Commodity(Fish, "Fish", SPOIL_HIGH),
            new Commodity(Meat, "Meat", SPOIL_HIGH),
            new Commodity(Bread, "Bread", SPOIL_MED),
            new Commodity(Wine, "Wine", SPOIL_LOW),
            new Commodity(Beer, "Beer", SPOIL_MED),

            new Commodity(Wood, "Wood", SPOIL_LOW),
            new Commodity(Timber, "Timber", SPOIL_LOW),
            new Commodity(Barrel, "Barrel", SPOIL_LOW),
            new Commodity(Furniture, "Furniture", SPOIL_LOW),

            new Commodity(Cotton, "Cotton", SPOIL_LOW),
            new Commodity(Fabric, "Fabric", SPOIL_LOW),
            new Commodity(Clothes, "Clothes", SPOIL_LOW),
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
            new RecipeBuilder(Hops)
                .Consumes(UnskilledWork, 1)
                .Produces(Hops, 8)
                .Build(),
            new RecipeBuilder(Meat)
                .Consumes(UnskilledWork, 1)
                .Produces(Meat, 8)
                .Build(),
            new RecipeBuilder(Fish)
                .Consumes(UnskilledWork, 1)
                .Produces(Fish, 8)
                .Build(),
            new RecipeBuilder(Bread)
                .Consumes(UnskilledWork, 1)
                .Consumes(Grain, 1)
                .Produces(Bread, 4)
                .Build(),
            new RecipeBuilder(Wine)
                .Consumes(UnskilledWork, 1)
                .Consumes(Fruit, 1)
                .Consumes(Barrel, 1)
                .Produces(Wine, 2)
                .Build(),
            new RecipeBuilder(Beer)
                .Consumes(UnskilledWork, 1)
                .Consumes(Grain, 1)
                .Consumes(Hops, 1)
                .Consumes(Barrel, 1)
                .Produces(Beer, 4)
                .Build(),

            new RecipeBuilder(Wood)
                .Consumes(UnskilledWork, 1)
                .Produces(Wood, 16)
                .Build(),
            new RecipeBuilder(Timber)
                .Consumes(UnskilledWork, 1)
                .Consumes(Wood, 1)
                .Produces(Timber, 8)
                .Build(),
            new RecipeBuilder(Barrel)
                .Consumes(UnskilledWork, 1)
                .Consumes(Timber, 1)
                .Produces(Barrel, 8)
                .Build(),
            new RecipeBuilder(Furniture)
                .Consumes(UnskilledWork, 1)
                .Consumes(Timber, 1)
                .Produces(Furniture, 4)
                .Build(),

            new RecipeBuilder(Cotton)
                .Consumes(UnskilledWork, 1)
                .Produces(Cotton, 8)
                .Build(),
            new RecipeBuilder(Fabric)
                .Consumes(UnskilledWork, 1)
                .Consumes(Cotton, 1)
                .Produces(Fabric, 4)
                .Build(),
            new RecipeBuilder(Clothes)
                .Consumes(UnskilledWork, 1)
                .Consumes(Fabric, 1)
                .Produces(Clothes, 4)
                .Build(),
        }
            .ToDictionary(x => x.Key, x => x);

    }
}
