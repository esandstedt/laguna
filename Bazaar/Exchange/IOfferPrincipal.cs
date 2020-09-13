using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Exchange
{
    public interface IOfferPrincipal
    {
        void AddInventory(string commodity, double amount);
        void RemoveInventory(string commodity, double amount);
        void UpdatePriceModel(OfferType type, string commodity, bool success, double price = 0.0);
    }
}
