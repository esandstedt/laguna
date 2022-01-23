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
        public const string RawFood = "raw-food";
        public const string Food = "food";

        public const string Wood = "wood";
        public const string Timber = "timber";

        public static readonly Dictionary<string, Commodity> Commodities = new List<Commodity>
        {
            new Commodity(UnskilledWork, "UnskilledWork", SPOIL_ALL),
            new Commodity(RawFood, "RawFood", SPOIL_LOW),
            new Commodity(Food, "Food", SPOIL_MED),

            new Commodity(Wood, "Wood", SPOIL_LOW),
            new Commodity(Timber, "Timber", SPOIL_LOW),

        }
            .ToDictionary(x => x.Key, x => x);


        public static readonly Dictionary<string, Recipe> Recipes = new List<Recipe>
        {
            new RecipeBuilder(RawFood)
                .Consumes(UnskilledWork, 1)
                .Produces(RawFood, 1.5)
                .Build(),
            new RecipeBuilder(Food)
                .Consumes(UnskilledWork, 1)
                .Consumes(RawFood, 1)
                .Produces(Food, 1.5)
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
        }
            .ToDictionary(x => x.Key, x => x);

    }
}
