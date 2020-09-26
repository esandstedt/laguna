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

            var first = world.AddTown(
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0.25 },
                        { Constants.Fish, 4 },
                        { Constants.Apples, 0.01 },
                        { Constants.Oranges, 0.01 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0 },
                    }
                }
            );

            var second = world.AddTown(
                new Area
                {
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 4 },
                        { Constants.Fish, 0.05 },
                        { Constants.Apples, 0.5 },
                        { Constants.Oranges, 0.25 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0.5 },
                    }
                }
            );

            for (var i = 0; i < 2000; i++)
            {
                world.Step();

                var firstBread = first.Market.GetHistory(Constants.Bread).FirstOrDefault();
                var firstFish = first.Market.GetHistory(Constants.Fish).FirstOrDefault();
                var firstApples = first.Market.GetHistory(Constants.Apples).FirstOrDefault();
                var firstOranges = first.Market.GetHistory(Constants.Oranges).FirstOrDefault();
                var secondBread = second.Market.GetHistory(Constants.Bread).FirstOrDefault();
                var secondFish = second.Market.GetHistory(Constants.Fish).FirstOrDefault();
                var secondApples = second.Market.GetHistory(Constants.Apples).FirstOrDefault();
                var secondOranges = second.Market.GetHistory(Constants.Oranges).FirstOrDefault();

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
                var firstAgentCounts = first.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                var secondAgentCounts = second.Agents
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
