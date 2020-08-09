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
        public List<Agent> Agents { get; }
        public Market Market { get; }

        public Town(Area area)
        {
            this.Area = area;
            this.Agents = new List<Agent>();
            this.Market = new Market();
        }


        public void AddAgent(Agent agent)
        {
            this.Agents.Add(agent);
            this.Market.Agents.Add(agent);
        }

        public void RemoveAgent(Agent agent)
        {
            this.Agents.Remove(agent);
            this.Market.Agents.Remove(agent);
        }
    }
}
