using Bazaar.Example.ConsoleApp.Agents;
using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Town
    {

        public string Name { get; }
        public Area Area { get; }
        public List<Agent> Agents { get; } = new List<Agent>();
        public IMarket Market { get; } = new Market();

        private readonly double maxMoney;
        private readonly Random random = new Random();

        public Town(string name, Area area, int agentCount, double maxMoney)
        {
            this.Name = name;
            this.Area = area;
            this.maxMoney = maxMoney;

            this.AddAgents(agentCount);
        }

        public void Step()
        {
            this.ReplaceBankruptAgents();
            this.TaxAgents();
        }

        /*
        private void AddRandomAgent()
        {
            var agent = new List<Func<Agent>>
            {
                () => new Farmer(this),
                () => new Miller(this),
                () => new Baker(this),
                () => new Fisherman(this),
                () => new Orchardist(this),
                () => new Lumberjack(this),
                () => new Sawyer(this),
                () => new Miner(this),
                () => new Refiner(this),
                () => new Blacksmith(this),
            }
                .Random()
                .Invoke();

            this.Agents.Add(agent);
        }
         */

        private void ReplaceBankruptAgents()
        {
            var agentsToRemove = this.Agents
                .Where(x => x.Inventory.Get(Constants.Money) < 0)
                .ToList();

            foreach (var agent in agentsToRemove)
            {
                this.Agents.Remove(agent);
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
                }
            }
        }

        private void TaxAgents()
        {
            var totalMoney = this.Agents.Sum(x => x.Inventory.Get(Constants.Money));
            if (this.maxMoney < totalMoney)
            {
                var percent = this.maxMoney / totalMoney;
                foreach (var agent in this.Agents)
                {
                    agent.Inventory.Set(
                        Constants.Money,
                        percent * agent.Inventory.Get(Constants.Money)
                    );
                }
            }
        }
    }
}
