using Bazaar.Example.ConsoleApp.Agents;
using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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
                        { Constants.Fish, 2 },
                        { Constants.Apples, 0.01 },
                        { Constants.Oranges, 0.01 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0.5 },
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
                        { Constants.Apples, 0.8 },
                        { Constants.Oranges, 0.2 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0.5 },
                    }
                }
            );

            for (var i = 0; i < 2000; i++)
            {
                world.Step();

                var firstBread = first.Market.History[Constants.Bread].First();
                var firstFish = first.Market.History[Constants.Fish].First();
                var firstApples = first.Market.History[Constants.Apples].First();
                var firstOranges = first.Market.History[Constants.Oranges].First();
                var secondBread = second.Market.History[Constants.Bread].First();
                var secondFish = second.Market.History[Constants.Fish].First();
                var secondApples = second.Market.History[Constants.Apples].First();
                var secondOranges = second.Market.History[Constants.Oranges].First();

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
                var firstAgentCounts = first.Market.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                var secondAgentCounts = second.Market.Agents
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
