using Bazaar.Example.ConsoleApp.Agents;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Town
    {

        public string Name { get; }
        public List<Agent> Agents { get; } = new List<Agent>();
        public IMarket Market { get; } = new MarketImpl();

        public double Money { get; set; }

        private readonly Area area;
        private readonly Random random = new Random();

        public Town(string name, Area area, int agentCount)
        {
            this.Name = name;
            this.area = area;

            this.Money = 100 * agentCount;

            this.AddAgents(agentCount);
        }

        public double GetRatio(string commodity)
        {
            return this.area.Capacity[commodity];
        }

        public void Step()
        {
            this.TaxAgents();
            this.ReplaceBankruptAgents();
            this.AddAgents(10);
        }

        private void ReplaceBankruptAgents()
        {
            var agentsToRemove = this.Agents
                .Where(x => x.Inventory.Get(Constants.Money) < 0)
                .ToList();

            foreach (var agent in agentsToRemove)
            {
                this.Agents.Remove(agent);
                this.Money += agent.Inventory.Get(Constants.Money);
            }

            this.AddAgents(agentsToRemove.Count);
        }

        private void AddAgents(int amount)
        {
            var weightMap = new Dictionary<string, (int, double)>
            {
                {  "farmer", (1, 10) },
                {  "miller", (1, 10) },
                {  "baker", (1, 10) },
                {  "fisherman", (1, 10) },
                {  "orchardist", (1, 10) },
                {  "lumberjack", (1, 10)},
                {  "sawyer", (1, 10) },
                {  "miner", (1, 10) },
                {  "refiner", (1, 10) },
                {  "blacksmith", (1, 10) },
            };

            foreach (var agent in this.Agents)
            {
                var (count, money) = weightMap[agent.Type];

                weightMap[agent.Type] = (count + 1, money + agent.Inventory.Get(Constants.Money));
            }

            var weights = weightMap
                .Select(pair => new KeyValuePair<string, double>(pair.Key, pair.Value.Item2 / pair.Value.Item1))
                .ToList();

            var totalWeight = weights.Sum(x => x.Value);

            for (var i = 0; i < amount; i++)
            {
                if (this.Money < 100)
                {
                    break;
                }

                var weight = this.random.NextDouble() * totalWeight;
                var j = 0;
                for (; weights[j].Value < weight; j++)
                {
                    weight -= weights[j].Value;
                }

                switch (weights[j].Key)
                {
                    case "farmer":
                        this.Agents.Add(new Farmer(this));
                        break;
                    case "miller":
                        this.Agents.Add(new Miller(this));
                        break;
                    case "baker":
                        this.Agents.Add(new Baker(this));
                        break;
                    case "fisherman":
                        this.Agents.Add(new Fisherman(this));
                        break;
                    case "orchardist":
                        this.Agents.Add(new Orchardist(this));
                        break;
                    case "lumberjack":
                        this.Agents.Add(new Lumberjack(this));
                        break;
                    case "sawyer":
                        this.Agents.Add(new Sawyer(this));
                        break;
                    case "miner":
                        this.Agents.Add(new Miner(this));
                        break;
                    case "refiner":
                        this.Agents.Add(new Refiner(this));
                        break;
                    case "blacksmith":
                        this.Agents.Add(new Blacksmith(this));
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                this.Money -= 100;
            }
        }

        private void TaxAgents()
        {
            var totalMoney = this.Agents.Sum(x => x.Inventory.Get(Constants.Money)) + this.Money;

            foreach (var agent in this.Agents)
            {
                var money = agent.Inventory.Get(Constants.Money);
                var percent = Math.Clamp((money - 100) / (1000 - 100), 0.0, 1.0);

                var amount = Math.Max(1, percent * money);
                this.Money += amount;
                agent.Inventory.Remove(Constants.Money, amount);
            }
        }
    }
}
