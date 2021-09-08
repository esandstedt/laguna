using System;
using System.Collections.Generic;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class Recipe 
    {
        public string Key { get; set; }
        public List<(string Commodity, double Amount)> Consumes { get; set; } 
        public string Produces { get; set; }
    }
}
