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

        public Route(Town fst, Town snd, int agentCount)
        {
            this.First = fst;
            this.Second = snd;

            for (var i=0; i<agentCount; i++)
            {
                // this.AddRandomAgent();
            }
        }
    }
}
