using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Constants
    {
        public const string Money = Bazaar.Constants.Money;

        public const string Grain = "grain";
        public const string Flour = "flour";
        public const string Bread = "bread";
        public const string Fish = "fish";
        public const string Apples = "apples";
        public const string Oranges = "oranges";

        public const string Logs = "logs";
        public const string Planks = "planks";

        public const string Ore = "ore";
        public const string Metal = "metal";
        public const string Tools = "tools";

        public readonly static List<string> TradableCommodities = new List<string>
        {
            Grain,
            Flour,
            Bread,
            Fish,
            Apples,
            Oranges,
            //Logs,
            Planks,
            //Ore,
            Metal,
            Tools
        };
    }
}
