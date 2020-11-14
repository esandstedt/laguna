using System;
using System.Collections.Generic;
using System.Linq;

namespace Bazaar.Example.ConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var world = new World();

            var gefion = world.AddTown(
                "Gefion",
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0.5 },
                        { Constants.Fish, 0.5 },
                        { Constants.Apples, 0.25 },
                        { Constants.Oranges, 0.25 },
                        { Constants.Logs, 0 },
                        { Constants.Ore, 0.5 },

                        { Constants.Flour, 1 },
                        { Constants.Bread, 1 },
                    }
                },
                500
            );

            var gnomeran = world.AddTown(
                "Gnomeran",
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0.5 },
                        { Constants.Fish, 0.5 },
                        { Constants.Apples, 0.5 },
                        { Constants.Oranges, 0.5 },
                        { Constants.Logs, 0 },
                        { Constants.Ore, 0 },

                        { Constants.Flour, 2 },
                        { Constants.Bread, 1 },
                    }
                },
                250
            );

            var thrane = world.AddTown(
                "Thrane",
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0 },
                        { Constants.Fish, 0 },
                        { Constants.Apples, 0 },
                        { Constants.Oranges, 0 },
                        { Constants.Logs, 0.5 },
                        { Constants.Ore, 1.5 },

                        { Constants.Flour, 0.25 },
                        { Constants.Bread, 1 },
                    }
                },
                1000
            );

            var unther = world.AddTown(
                "Unther",
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0 },
                        { Constants.Fish, 0.5 },
                        { Constants.Apples, 0 },
                        { Constants.Oranges, 0 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0.5 },

                        { Constants.Flour, 0.25 },
                        { Constants.Bread, 1 },
                    }
                },
                250
            );

            var tolm = world.AddTown(
                "Tolm",
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0.25 },
                        { Constants.Fish, 0.75 },
                        { Constants.Apples, 0 },
                        { Constants.Oranges, 0 },
                        { Constants.Logs, 0.25 },
                        { Constants.Ore, 0.75 },

                        { Constants.Flour, 0.25 },
                        { Constants.Bread, 1 },
                    }
                },
                250
            );

            var gefionGnomeranRoute = world.AddRoute(gefion, gnomeran, 25);
            var gefionThraneRoute = world.AddRoute(gefion, thrane, 50);
            var gnomeranThraneRoute = world.AddRoute(gnomeran, thrane, 25);
            var thraneUntherRoute = world.AddRoute(thrane, unther, 25);
            var untherTolmRoute = world.AddRoute(unther, tolm, 25);

            for (var i = 0; i < 2000; i++)
            {
                world.Step();

                var firstBread = gefion.Market.GetHistory(Constants.Bread).FirstOrDefault();
                var firstFish = gefion.Market.GetHistory(Constants.Fish).FirstOrDefault();
                var firstApples = gefion.Market.GetHistory(Constants.Apples).FirstOrDefault();
                var firstOranges = gefion.Market.GetHistory(Constants.Oranges).FirstOrDefault();
                var secondBread = tolm.Market.GetHistory(Constants.Bread).FirstOrDefault();
                var secondFish = tolm.Market.GetHistory(Constants.Fish).FirstOrDefault();
                var secondApples = tolm.Market.GetHistory(Constants.Apples).FirstOrDefault();
                var secondOranges = tolm.Market.GetHistory(Constants.Oranges).FirstOrDefault();

                Console.WriteLine(
                    "{0,5} || {1,4} {2,6:F2} | {3,4} {4,6:F2} | {5,4} {6,6:F2} {7,4} {8,6:F2} || {9,4} {10,6:F2} | {11,4} {12,6:F2} | {13,4} {14,6:F2} | {15,4} {16,6:F2}", 
                    i,
                    (int)firstBread.AmountTraded,
                    firstBread.AveragePrice,
                    (int)firstFish.AmountTraded,
                    firstFish.AveragePrice,
                    (int)firstApples.AmountTraded,
                    firstApples.AveragePrice,
                    (int)firstOranges.AmountTraded,
                    firstOranges.AveragePrice,
                    (int)secondBread.AmountTraded,
                    secondBread.AveragePrice,
                    (int)secondFish.AmountTraded,
                    secondFish.AveragePrice,
                    (int)secondApples.AmountTraded,
                    secondApples.AveragePrice,
                    (int)secondOranges.AmountTraded,
                    secondOranges.AveragePrice
                );
            }

            {
                var gefionAgentCounts = gefion.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                var gnomeranAgentCounts = gnomeran.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                var thraneAgentCounts = thrane.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                { }

            }

        }
    }
}
