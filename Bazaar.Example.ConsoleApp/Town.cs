using Bazaar.Example.ConsoleApp.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Town
    {

        public Area Area { get; }
        public List<Agent> Agents { get; } = new List<Agent>();
        public Market Market { get; } = new Market();

        private readonly double maxMoney;

        public Town(Area area, int agentCount, double maxMoney)
        {
            this.Area = area;

            this.maxMoney = maxMoney;

            for (var i=0; i<agentCount; i++)
            {
                this.AddRandomAgent();
            }
        }

        public void Step()
        {
            foreach (var agent in this.Agents)
            {
                agent.Step();
            }

            this.Market.ResolveOffers();

            this.ReplaceBankruptAgents();
            this.TaxAgents();
        }

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

        private void ReplaceBankruptAgents()
        {
            var agents = this.Agents
                .Where(x => x.Inventory.Get(Constants.Money) < 0)
                .ToList();

            foreach (var agent in agents)
            {
                this.Agents.Remove(agent);
            }

            for (var i = 0; i < agents.Count; i++)
            {
                this.AddRandomAgent();
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
