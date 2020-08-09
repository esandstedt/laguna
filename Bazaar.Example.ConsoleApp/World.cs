﻿using Bazaar.Example.ConsoleApp.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public class World
    {
        private readonly static int AGENT_COUNT = 500;
        private readonly static double MAX_MONEY = AGENT_COUNT * 100;

        public IEnumerable<Agent> Agents => this.Towns.SelectMany(x => x.Market.Agents);
        public List<Town> Towns { get; set; } = new List<Town>();

        public Town AddTown(Area area)
        {
            var town = new Town(area);

            for (var i=0; i<AGENT_COUNT; i++)
            {
                this.AddRandomAgent(town);
            }

            this.Towns.Add(town);

            return town;
        }

        private void AddRandomAgent(Town town)
        {
            var agent = new List<Func<Agent>>
            {
                () => new Farmer(town),
                () => new Miller(town),
                () => new Baker(town),
                () => new Fisherman(town),
                () => new Orchardist(town),
                () => new Lumberjack(town),
                () => new Sawyer(town),
                () => new Miner(town),
                () => new Refiner(town),
                () => new Blacksmith(town),
            }
                .Random()
                .Invoke();

            town.AddAgent(agent);
        }


        public void Step()
        {
            foreach (var agent in this.Agents)
            {
                agent.Step();
            }

            foreach (var town in this.Towns)
            {
                town.Market.Step();
                this.ReplaceBankruptAgents(town);
                this.TaxAgents(town);
            }

        }

        private void ReplaceBankruptAgents(Town town)
        {
            var agents = town.Agents
                .Where(x => x.Inventory.Get(Constants.Money) < 0)
                .ToList();

            foreach (var agent in agents)
            {
                town.RemoveAgent(agent);
            }

            for (var i = 0; i < agents.Count; i++)
            {
                this.AddRandomAgent(town);
            }
        }

        private void TaxAgents(Town town)
        {
            var totalMoney = town.Market.Agents.Sum(x => x.Inventory.Get(Constants.Money));
            if (MAX_MONEY < totalMoney)
            {
                var percent = MAX_MONEY / totalMoney;
                foreach (var agent in town.Market.Agents)
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