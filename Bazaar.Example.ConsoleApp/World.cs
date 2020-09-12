using Bazaar.Example.ConsoleApp.Agents;
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

        public List<Town> Towns { get; } = new List<Town>();

        public Town AddTown(Area area)
        {
            var town = new Town(area, AGENT_COUNT, MAX_MONEY);
            this.Towns.Add(town);
            return town;
        }

        public void Step()
        {
            foreach (var town in this.Towns)
            {
                town.Step();
            }
        }

    }

}
