using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Exchange
{
    public class Offer
    {
        public IOfferPrincipal Principal { get; }
        public OfferType Type { get; }
        public string Commodity { get; }
        public double Price { get; }
        public double Amount { get; }
        public OfferResult Result { get; set; }

        public Offer(IOfferPrincipal principal, OfferType type, string commodity, double price, double amount)
        {
            Principal = principal;
            Type = type;
            Commodity = commodity;
            Price = price;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"Offer: {Type} {Amount:F1} {Commodity} at {Price:F2}";
        }
    }

    public class OfferResult
    {
        public bool Success { get; }
        public double Price { get; }

        public OfferResult(bool success, double price = default)
        {
            Success = success;
            Price = price;
        }
    }
}
