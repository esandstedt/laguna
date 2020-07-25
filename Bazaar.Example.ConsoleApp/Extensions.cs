using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Example.ConsoleApp
{
    public static class Extensions
    {
        public static T Random<T>(this List<T> list)
        {
            return list[(int)(new Random().NextDouble() * list.Count)];
        }
    }
}
