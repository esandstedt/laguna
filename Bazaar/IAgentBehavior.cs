using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar
{
    public interface IAgentBehavior
    {
        void Perform();
        IEnumerable<Offer> GenerateOffers();
    }
}
