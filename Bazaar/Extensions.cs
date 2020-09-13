using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bazaar
{
    public static class Extensions
    {
        public static IEnumerable<Offer> Split(this Offer offer, double splitAmount)
        {
            var amount = offer.Amount;
            while (splitAmount < amount)
            {
                amount -= splitAmount;

                var clone = offer.Clone();
                clone.Amount = splitAmount;
                yield return clone;
            }

            {
                var clone = offer.Clone();
                clone.Amount = amount;
                yield return clone;
            }
        }
    }
}
