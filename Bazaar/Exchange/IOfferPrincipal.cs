using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Exchange
{
    public interface IOfferPrincipal
    {
        Inventory Inventory { get; }
    }
}
