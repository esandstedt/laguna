using System;
using System.Collections.Generic;
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
        }
    }

    public class RouteHistory
    {
        public Dictionary<string, double> FirstExports = new Dictionary<string, double>();
        public Dictionary<string, double> SecondExports = new Dictionary<string, double>();
    }
}
