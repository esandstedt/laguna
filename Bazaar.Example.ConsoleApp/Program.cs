using Bazaar.Example.ConsoleApp.Agents;
using Bazaar.Example.ConsoleApp.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Bazaar.Example.ConsoleApp
{
    class Program
    {

        private static Random Random = new Random();

        public static void Main(string[] args)
        {
            var market = new Market();

            var agentCount = 200;
            var maxMoney = agentCount * 100;

            for (var i=0; i<agentCount; i++)
            {
                market.Agents.Add(CreateRandomAgent(market));
            }

            for (var i = 0; i < 1000; i++)
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

                /*
                var commodityHistory = market.History.Keys
                    .Select(x => market.History[x].Last())
                    .ToList();

                var commodityDemands = commodityHistory
                    .Select(x => new
                    {
                        x.Commodity,
                        //Demand = Math.Max(1, x.AmountToBuy - x.AmountToSell)
                        Demand = ((x.AmountToBuy + 1) / (x.AmountToSell + 1))
                    })
                    .OrderByDescending(a => a.Demand)
                    .ToList();

                var totalDemand = commodityDemands.Sum(x => x.Demand);

                for (var j=0; j<bankruptAgents.Count; j++)
                {

                    string commodity = commodityDemands.First().Commodity;
                    var selection = Random.NextDouble() * totalDemand;
                    foreach (var x in commodityDemands)
                    {
                        if (selection < x.Demand)
                        {
                            commodity = x.Commodity;
                            break;
                        }
                        selection -= x.Demand;
                    }

                    switch (commodity)
                    {
                        case Constants.Wheat:
                            market.Agents.Add(new Farmer(market));
                            break;
                        case Constants.Bread:
                            market.Agents.Add(new Baker(market));
                            break;
                        case Constants.Wood:
                            market.Agents.Add(new Woodcutter(market));
                            break;
                        case Constants.Ore:
                            market.Agents.Add(new Miner(market));
                            break;
                        case Constants.Metal:
                            market.Agents.Add(new Refiner(market));
                            break;
                        case Constants.Tools:
                            market.Agents.Add(new Blacksmith(market));
                            break;
                    }
                }
                 */

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

                var wheat = market.History[Constants.Wheat].Last();
                var bread = market.History[Constants.Bread].Last();
                var fish = market.History[Constants.Fish].Last();
                var wood = market.History[Constants.Wood].Last();
                var tools = market.History[Constants.Tools].Last();

                Console.WriteLine(
                    "{0,5}: {1,3} | {2,4} {3,6:F2} | {4,4} {5,6:F2} | {6,4} {7,6:F2} | {8,4} {9,6:F2} | {10,4} {11,6:F2}", 
                    i,
                    bankruptAgents.Count,
                    (int)wheat.AmountToSell,
                    wheat.AveragePrice,
                    (int)bread.AmountToSell,
                    bread.AveragePrice,
                    (int)fish.AmountToSell,
                    fish.AveragePrice,
                    (int)wood.AmountToSell,
                    wood.AveragePrice,
                    (int)tools.AmountToSell,
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
            switch (Random.Next(7))
            {
                case 0: return new Farmer(market);
                case 1: return new Baker(market);
                case 2: return new Fisherman(market);
                case 3: return new Woodcutter(market);
                case 4: return new Miner(market);
                case 5: return new Refiner(market);
                case 6: return new Blacksmith(market);
                default: throw new InvalidOperationException();
            }
        }
    }
}
