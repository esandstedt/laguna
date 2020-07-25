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
            var first = new Town(
                new Area
                { 
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 0.25 },
                        { Constants.Fish, 2 },
                        { Constants.Apples, 0.05 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0.5 },
                    }
                }
            );

            var second = new Town(
                new Area
                { 
                    Production = new Dictionary<string, double>
                    {
                        { Constants.Grain, 4 },
                        { Constants.Fish, 0.25 },
                        { Constants.Apples, 0.75 },
                        { Constants.Logs, 1 },
                        { Constants.Ore, 0.5 },
                    }
                }
            );

            for (var i = 0; i < 2000; i++)
            {
                first.Step();
                second.Step();

                var firstBread = first.Market.History[Constants.Bread].First();
                var firstFish = first.Market.History[Constants.Fish].First();
                var firstApples = first.Market.History[Constants.Apples].First();
                var secondBread = second.Market.History[Constants.Bread].First();
                var secondFish = second.Market.History[Constants.Fish].First();
                var secondApples = second.Market.History[Constants.Apples].First();

                Console.WriteLine(
                    "{0,5}: {1,3} || {2,4} {3,6:F2} | {4,4} {5,6:F2} | {6,4} {7,6:F2} || {8,4} {9,6:F2} | {10,4} {11,6:F2} | {12,4} {13,6:F2}", 
                    i,
                    0,
                    (int)firstBread.AmountTraded,
                    firstBread.AveragePrice,
                    (int)firstFish.AmountTraded,
                    firstFish.AveragePrice,
                    (int)firstApples.AmountTraded,
                    firstApples.AveragePrice,
                    (int)secondBread.AmountTraded,
                    secondBread.AveragePrice,
                    (int)secondFish.AmountTraded,
                    secondFish.AveragePrice,
                    (int)secondApples.AmountTraded,
                    secondApples.AveragePrice
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
