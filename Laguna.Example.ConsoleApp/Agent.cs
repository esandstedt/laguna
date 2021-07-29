using Laguna.Agent;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public abstract class Agent : IMarketAgent
    {
        public Inventory Inventory { get; set; }
        public PriceBeliefs PriceBeliefs { get; set; }

        public Agent()
        {
            this.Inventory = new Inventory();
            this.PriceBeliefs = new PriceBeliefs();
        }

        public abstract IEnumerable<Offer> CreateOffers();

        protected IEnumerable<Offer> CreateOffers(OfferType type, string commodity, double amount)
        {
            while (0 < amount)
            {
                yield return new Offer(
                    type,
                    commodity,
                    this.PriceBeliefs.GetRandom(commodity),
                    Math.Min(1, amount)
                );

                amount -= 1;
            }
        }

        public virtual void HandleOfferResults(IEnumerable<Offer> offers)
        {
            foreach (var offer in offers)
            {
                var amount = offer.Results.Sum(x => x.Amount);
                var money = offer.Results.Sum(x => x.Amount * x.Price);

                if (offer.Type == OfferType.Buy)
                {
                    this.Inventory.Add(offer.Commodity, amount);
                    this.Inventory.Remove(Constants.Money, money);
                }
                else if (offer.Type == OfferType.Sell)
                {
                    this.Inventory.Add(Constants.Money, money);
                    this.Inventory.Remove(offer.Commodity, amount);
                }
            }

            this.PriceBeliefs.Update(offers);
        }
    }
}
