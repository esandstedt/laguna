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

            var alpha = world.AddTown(
                "Alpha",
                new Area
                {
                    Capacity = new Dictionary<string, double>
                    {
                        { Constants.Grain, 1.0 },
                        { Constants.Fish, 0.01 },
                        { Constants.Apples, 0.01 },
                        { Constants.Oranges, 0.01 },
                        { Constants.Logs, 1.0 },
                        { Constants.Ore, 1.0 },

                        { Constants.Flour, 2.0 },
                        { Constants.Bread, 2.0 },
                    }
                },
                200
            );

            var bravo = world.AddTown(
                "Bravo",
                new Area
                {
                    Capacity = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0.25 },
                        { Constants.Fish, 1.0 },
                        { Constants.Apples, 0.5 },
                        { Constants.Oranges, 0.125 },
                        { Constants.Logs, 1.0 },
                        { Constants.Ore, 1.0 },

                        { Constants.Flour, 1.0 },
                        { Constants.Bread, 1.0 },
                    }
                },
                200
            );

            world.AddRoute(alpha, bravo, 20);

            for (var i = 0; i < 2000; i++)
            {
                world.Step();

                var firstBread = alpha.Market.History.GetValueOrDefault(Constants.Bread);
                var firstFish = alpha.Market.History.GetValueOrDefault(Constants.Fish);
                var firstApples = alpha.Market.History.GetValueOrDefault(Constants.Apples);
                var firstOranges = alpha.Market.History.GetValueOrDefault(Constants.Oranges);
                var secondBread = bravo.Market.History.GetValueOrDefault(Constants.Bread);
                var secondFish = bravo.Market.History.GetValueOrDefault(Constants.Fish);
                var secondApples = bravo.Market.History.GetValueOrDefault(Constants.Apples);
                var secondOranges = bravo.Market.History.GetValueOrDefault(Constants.Oranges);

                Console.WriteLine(
                    "{0,5} || {1,4:N0} {2,6:F2} | {3,4:N0} {4,6:F2} | {5,4:N0} {6,6:F2} {7,4:N0} {8,6:F2} || {9,4:N0} {10,6:F2} | {11,4:N0} {12,6:F2} | {13,4:N0} {14,6:F2} | {15,4:N0} {16,6:F2}", 
                    i,
                    firstBread?.AmountTraded,
                    firstBread?.AveragePrice,
                    firstFish?.AmountTraded,
                    firstFish?.AveragePrice,
                    firstApples?.AmountTraded,
                    firstApples?.AveragePrice,
                    firstOranges?.AmountTraded,
                    firstOranges?.AveragePrice,
                    secondBread?.AmountTraded,
                    secondBread?.AveragePrice,
                    secondFish?.AmountTraded,
                    secondFish?.AveragePrice,
                    secondApples?.AmountTraded,
                    secondApples?.AveragePrice,
                    secondOranges?.AmountTraded,
                    secondOranges?.AveragePrice
                );
            }

            {
                var alphaAgentCounts = alpha.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                var bravoAgentCounts = bravo.Agents
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
