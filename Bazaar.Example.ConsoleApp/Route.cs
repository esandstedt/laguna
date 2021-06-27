using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class Route
    {
        public Town First { get; }
        public Town Second { get; }
        public List<IAgent> Agents { get; } = new List<IAgent>();

        public RouteHistory History { get; private set; } = new RouteHistory();

        public Route(Town fst, Town snd, int agentCount)
        {
            this.First = fst;
            this.Second = snd;

            for (var i=0; i<agentCount; i++)
            {
                this.Agents.Add(new Trader(this));
            }
        }

        public void Step()
        {
            this.History = new RouteHistory();
            this.TaxAgents();
        }

        private void TaxAgents()
        {
            var totalMoney = this.Agents
                .Cast<Trader>()
                .Sum(x => x.First.BuyInventory.Get(Constants.Money) + x.Second.BuyInventory.Get(Constants.Money));

            foreach (var agent in this.Agents.Cast<Trader>())
            {
                {
                    var money = agent.First.BuyInventory.Get(Constants.Money);
                    var percent = Math.Clamp((money - 100) / (1000 - 100), 0.0, 1.0);
                    var amount =  percent * money;
                    this.First.Money += amount;
                    agent.First.BuyInventory.Remove(Constants.Money, amount);
                }

                {
                    var money = agent.Second.BuyInventory.Get(Constants.Money);
                    var percent = Math.Clamp((money - 100) / (1000 - 100), 0.0, 1.0);
                    var amount = percent * money;
                    this.Second.Money += amount;
                    agent.Second.BuyInventory.Remove(Constants.Money, amount);
                }
            }
        }
    }

    public class RouteHistory
    {
        public Dictionary<string, double> FirstExports = new Dictionary<string, double>();
        public Dictionary<string, double> SecondExports = new Dictionary<string, double>();
    }
}
