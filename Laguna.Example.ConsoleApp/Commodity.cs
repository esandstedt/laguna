using System;
using System.Collections.Generic;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class Commodity
    {
        public string Key { get; set; }
        public string Display { get; set; }
        public double SpoilRate { get; set; }
        public Commodity(string key, string display, double spoilRate)
        {
            this.Key = key;
            this.Display = display;
            this.SpoilRate = spoilRate;
        }
    }
}
