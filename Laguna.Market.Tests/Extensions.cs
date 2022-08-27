using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laguna.Market.Tests
{
    public static class Extensions
    {
        public static MarketImpl AddOffer(this MarketImpl market, Offer offer)
        {
            market.Add(new MarketAgent(offer));

            return market;
        }

        private class MarketAgent : IMarketAgent
        {
            private readonly Offer offer;

            public MarketAgent(Offer offer)
            {
                this.offer = offer;
            }

            public IEnumerable<Offer> CreateOffers()
            {
                return new List<Offer> { offer };
            }

            public void HandleOfferResults(IEnumerable<Offer> offers)
            {

            }
        }
    }


}
