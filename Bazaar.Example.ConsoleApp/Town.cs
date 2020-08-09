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
        public Area Area { get; set; }
        public Market Market { get; set; }

        public Town(Area area)
        {
            this.Area = area;
            this.Market = new Market();
        }
    }
}
