using System;
using System.Collections.Generic;
using System.Text;

namespace Laguna.Market
{
    public enum OfferType
    {
        Buy,
        Sell
    }

    public class Offer
    {
        public OfferType Type { get; }
        public string Commodity { get; }
        public double Price { get; }
        public double Amount { get; }
        public List<OfferResult> Results { get; }

        public Offer(OfferType type, string commodity, double price, double amount)
        {
            Type = type;
            Commodity = commodity;
            Price = price;
            Amount = amount;
            Results = new List<OfferResult>();
        }

        public override string ToString()
        {
            return $"Offer: {Type} {Amount:F1} {Commodity} at {Price:F2}";
        }

    }
}
