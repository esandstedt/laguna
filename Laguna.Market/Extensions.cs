using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Laguna
{
    public static class Extensions
    {
        public static IEnumerable<Offer> Split(this Offer offer, double splitAmount)
        {
            var amount = offer.Amount;

            while (splitAmount < amount)
            {
                amount -= splitAmount;

                yield return new Offer(
                    offer.Type,
                    offer.Commodity,
                    offer.Price,
                    splitAmount
                );
            }

            yield return new Offer(
                offer.Type,
                offer.Commodity,
                offer.Price,
                amount
            );
        }

        private static Random random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n -= 1;
                var k = random.Next(n + 1);
                var v = list[k];
                list[k] = list[n];
                list[n] = v;
            }
        }
    }
}
