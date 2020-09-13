using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Exchange
{
    public class Offer
    {
        public IOfferPrincipal Principal { get; set; }
        public OfferType Type { get; set; }
        public string Commodity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }

        public Offer(IOfferPrincipal principal, OfferType type, string commodity, double price, double amount)
        {
            Principal = principal;
            Type = type;
            Commodity = commodity;
            Price = price;
            Amount = amount;
        }

        public Offer Clone()
        {
            return new Offer(
                this.Principal,
                this.Type,
                this.Commodity,
                this.Price,
                this.Amount
            );
        }

        public override string ToString()
        {
            return $"Offer: {Type} {Amount:F1} {Commodity} at {Price:F2}";
        }
    }
}
