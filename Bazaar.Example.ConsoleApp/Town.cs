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

        private readonly static int AGENT_COUNT = 500;
        private readonly static double MAX_MONEY = AGENT_COUNT * 100;

        public Area Area { get; set; }
        public Market Market { get; set; }

        public Town(Area area)
        {
            this.Area = area;
            this.Market = new Market();

            for (var i=0; i<AGENT_COUNT; i++)
            {
                this.AddRandomAgent();
            }
        }

        private void AddRandomAgent()
        {
            var agent = new List<Func<Agent>>
            {
                () => new Farmer(this.Market, this.Area),
                () => new Miller(this.Market),
                () => new Baker(this.Market),
                () => new Fisherman(this.Market, this.Area),
                () => new Orchardist(this.Market, this.Area),
                () => new Lumberjack(this.Market, this.Area),
                () => new Sawyer(this.Market),
                () => new Miner(this.Market, this.Area),
                () => new Refiner(this.Market),
                () => new Blacksmith(this.Market),
            }
                .Random()
                .Invoke();

            this.Market.Agents.Add(agent);
        }


        public void Step()
        {
            this.Market.Step();
            this.ReplaceBankruptAgents();
            this.TaxAgents();
        }

        private void ReplaceBankruptAgents()
        {
            var agents = this.Market.Agents
                .Where(x => x.Inventory.Get(Constants.Money) < 0)
                .ToList();

            foreach (var agent in agents)
            {
                this.Market.Agents.Remove(agent);
            }

            for (var i = 0; i < agents.Count; i++)
            {
                this.AddRandomAgent();
            }
        }

        private void TaxAgents()
        {
            var totalMoney = this.Market.Agents.Sum(x => x.Inventory.Get(Constants.Money));
            if (MAX_MONEY < totalMoney)
            {
                var percent = MAX_MONEY / totalMoney;
                foreach (var agent in this.Market.Agents)
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
