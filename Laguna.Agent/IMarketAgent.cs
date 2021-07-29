using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laguna.Agent
{
    public interface IMarketAgent
    {
        IEnumerable<Offer> CreateOffers();
        void HandleOfferResults(IEnumerable<Offer> offers);
    }
}
