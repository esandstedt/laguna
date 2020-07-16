using Bazaar.Example.ConsoleApp.Agents;
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

            var agentCount = 1000;
            var maxMoney = agentCount * 50;

            for (var i=0; i<agentCount; i++)
            {
                market.Agents.Add(CreateRandomAgent());
            }

            for (var i = 0; i < 5000; i++)
            {
                market.Step();

                var bankruptAgents = market.Agents
                    .Where(x => x.Inventory.Get("money") < 0)
                    .ToList();

                Console.WriteLine("{0,5}: {1}", i, bankruptAgents.Count);

                foreach (var agent in bankruptAgents)
                {
                    market.Agents.Remove(agent);
                }

                var commodityHistory = market.History.Keys
                    .Select(x => market.History[x].Last())
                    .ToList();

                var commodityDemands = commodityHistory
                    .Select(x => new
                    {
                        x.Commodity,
                        Demand = Math.Max(1, x.AmountToBuy - x.AmountToSell)
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
                        case "food":
                            market.Agents.Add(new Farmer());
                            break;
                        case "wood":
                            market.Agents.Add(new Woodcutter());
                            break;
                        case "ore":
                            market.Agents.Add(new Miner());
                            break;
                        case "metal":
                            market.Agents.Add(new Refiner());
                            break;
                        case "tools":
                            market.Agents.Add(new Blacksmith());
                            break;
                    }
                }

                var totalMoney = market.Agents.Sum(x => x.Inventory.Get("money"));
                if (maxMoney < totalMoney)
                {
                    var percent = maxMoney / totalMoney;
                    foreach (var agent in market.Agents)
                    {
                        agent.Inventory.Set(
                            "money",
                            percent * agent.Inventory.Get("money")
                        );
                    }
                }
            }

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

        private static Agent CreateRandomAgent()
        {
            switch (Random.Next(5))
            {
                case 0: return new Farmer();
                case 1: return new Woodcutter();
                case 2: return new Miner();
                case 3: return new Refiner();
                case 4: return new Blacksmith();
                default: throw new InvalidOperationException();
            }
        }
    }
}
