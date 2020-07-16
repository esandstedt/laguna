using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
    public class Offer
    {
        public Agent Agent { get; set; }
        public OfferType Type { get; set; }
        public string Commodity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }

        public override string ToString()
        {
            return $"Offer: {Type} {Amount:F1} {Commodity} at {Price:F2}";
        }
    }

    public enum OfferType
    {
        Buy,
        Sell
    }
}
