using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Exchange
{
    public class OfferResult
    {
        public double Amount { get; }
        public double Price { get; }

        public OfferResult(double amount, double price)
        {
            Amount = amount;
            Price = price;
        }
    }
}
