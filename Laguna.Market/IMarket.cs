using System;
using System.Collections.Generic;
using System.Text;

namespace Laguna.Market
{
    public interface IMarket
    {
        Dictionary<string, MarketHistory> History { get; }

        void AddOffer(Offer offer);
        void ResolveOffers();
    }
}
