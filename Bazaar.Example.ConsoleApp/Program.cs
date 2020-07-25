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

        private static Random Random = new Random();

        public static void Main(string[] args)
        {
            var market = new Market();

            var agentCount = 500;
            var maxMoney = agentCount * 100;

            for (var i=0; i<agentCount; i++)
            {
                market.Agents.Add(CreateRandomAgent(market));
            }

            for (var i = 0; i < 2000; i++)
            {
                market.Step();

                var bankruptAgents = market.Agents
                    .Where(x => x.Inventory.Get(Constants.Money) < 0)
                    .ToList();

                foreach (var agent in bankruptAgents)
                {
                    market.Agents.Remove(agent);
                }

                for (var j = 0; j < bankruptAgents.Count; j++)
                {
                    market.Agents.Add(CreateRandomAgent(market));
                }

                var totalMoney = market.Agents.Sum(x => x.Inventory.Get(Constants.Money));
                if (maxMoney < totalMoney)
                {
                    var percent = maxMoney / totalMoney;
                    foreach (var agent in market.Agents)
                    {
                        agent.Inventory.Set(
                            Constants.Money,
                            percent * agent.Inventory.Get(Constants.Money)
                        );
                    }
                }

                var grain = market.History[Constants.Grain].Last();
                var flour = market.History[Constants.Flour].Last();
                var bread = market.History[Constants.Bread].Last();
                var fish = market.History[Constants.Fish].Last();
                var apples = market.History[Constants.Apples].Last();
                var logs = market.History[Constants.Logs].Last();
                var planks = market.History[Constants.Planks].Last();
                var tools = market.History[Constants.Tools].Last();

                Console.WriteLine(
                    "{0,5}: {1,3} | {2,4} {3,6:F2} | {4,4} {5,6:F2} | {6,4} {7,6:F2} | {8,4} {9,6:F2} | {10,4} {11,6:F2} | {12,4} {13,6:F2}", 
                    i,
                    bankruptAgents.Count,
                    (int)grain.AmountTraded,
                    grain.AveragePrice,
                    (int)flour.AmountTraded,
                    flour.AveragePrice,
                    (int)bread.AmountTraded,
                    bread.AveragePrice,
                    (int)fish.AmountTraded,
                    fish.AveragePrice,
                    (int)apples.AmountTraded,
                    apples.AveragePrice,
                    (int)tools.AmountTraded,
                    tools.AveragePrice
                );
            }

            {
                var agentCounts = market.Agents
                    .GroupBy(x => x.Type)
                    .Select(x => new
                    {
                        Type = x.Key,
                        Count = x.Count()
                    })
                    .ToList();

                var history = market.History.Keys
                    .Select(x => market.History[x].Last())
                    .ToList();

                { }

            }

        }

        private static Agent CreateRandomAgent(Market market)
        {
            return new List<Func<Agent>>
            {
                () => new Farmer(market),
                () => new Miller(market),
                () => new Baker(market),
                () => new Fisherman(market),
                () => new Orchardist(market),
                () => new Lumberjack(market),
                () => new Sawyer(market),
                () => new Miner(market),
                () => new Refiner(market),
                () => new Blacksmith(market),
            }
                .Random()
                .Invoke();
        }
    }
}
