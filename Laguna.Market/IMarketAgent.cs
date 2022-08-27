using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laguna.Market
{
    public interface IMarketAgent
    {
        IEnumerable<Offer> CreateOffers();
        void HandleOfferResults(IEnumerable<Offer> offers);
    }
}
