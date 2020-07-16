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

            var agentCount = 50;
            var maxMoney = agentCount * 50;

            for (var i=0; i<agentCount; i++)
            {
                market.Agents.Add(CreateRandomAgent(market));
            }

            for (var i = 0; i < 500; i++)
            {
                market.Step();

                var bankruptAgents = market.Agents
                    .Where(x => x.Inventory.Get("money") < 0)
                    .ToList();

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
                            market.Agents.Add(new Farmer(market));
                            break;
                        case "wood":
                            market.Agents.Add(new Woodcutter(market));
                            break;
                        case "ore":
                            market.Agents.Add(new Miner(market));
                            break;
                        case "metal":
                            market.Agents.Add(new Refiner(market));
                            break;
                        case "tools":
                            market.Agents.Add(new Blacksmith(market));
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

                Console.WriteLine("{0,5}: {1,3}", i, bankruptAgents.Count);
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

            var beliefs = market.Agents
                .Where(x => x.Type == "blacksmith")
                .Select(x => x.PriceBeliefs.Get("tools"))
                .ToList();

            var total = beliefs.Aggregate((acc, x) => (acc.Item1 + x.Item1, acc.Item2 + x.Item2));
            var average = (total.Item1 / beliefs.Count, total.Item2 / beliefs.Count);

            { }

        }

        private static Agent CreateRandomAgent(Market market)
        {
            switch (Random.Next(5))
            {
                case 0: return new Farmer(market);
                case 1: return new Woodcutter(market);
                case 2: return new Miner(market);
                case 3: return new Refiner(market);
                case 4: return new Blacksmith(market);
                default: throw new InvalidOperationException();
            }
        }
    }
}
