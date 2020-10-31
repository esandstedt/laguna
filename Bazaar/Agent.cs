using Bazaar.Exchange;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bazaar
{

    public interface IAgent
    {
        string Type { get; }
        Inventory Inventory { get; }
        void Perform();
        void SubmitOffers();
        void HandleOfferResults();
    }

    public abstract class Agent : IOfferPrincipal, IAgent
    {

        private static readonly double MINIMUM_PRICE = 1;
        private static readonly double MAXIMUM_PRICE = 100;

        public string Type { get; }
        public List<AgentBehavior> Behaviors { get; }
        public Market Market { get; }
        public Inventory Inventory { get; } 
        public PriceBeliefs PriceBeliefs { get; }
        public CostBeliefs CostBeliefs { get; }

        private readonly List<Offer> offers; 

        public Agent(string type, Market market)
        {
            this.Type = type;
            this.Market = market;
            this.Behaviors = new List<AgentBehavior>();
            this.Inventory = new Inventory();
            this.PriceBeliefs = new PriceBeliefs(MINIMUM_PRICE, MAXIMUM_PRICE);
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs, MINIMUM_PRICE);

            this.offers = new List<Offer>();

            this.PriceBeliefs.Initialize(market);
        }

        public void Perform()
        {
            this.CostBeliefs.Begin();

            foreach (var behavior in this.Behaviors)
            {
                behavior.Perform();
            }

            this.CostBeliefs.End();
        }

        public void SubmitOffers()
        {
            Debug.Assert(this.offers.Count == 0, "Offers list is not empty.");

            var offers = this.Behaviors
                .SelectMany(x => x.GenerateOffers())
                .Where(x => x != null)
                .ToList();

            var money = this.Inventory.Get(Constants.Money);

            var buyOffers = offers.Where(x => x.Type == OfferType.Buy)
                .SelectMany(x => x.Split(1))
                .OrderBy(x => Guid.NewGuid());

            foreach (var offer in buyOffers)
            {
                money -= offer.Price * offer.Amount;

                if (money < 0)
                {
                    break;
                }

                this.offers.Add(offer);
                this.Market.AddOffer(offer);
            }

            var sellOffers = offers.Where(x => x.Type == OfferType.Sell)
                .SelectMany(x => x.Split(1));

            foreach (var offer in sellOffers)
            {
                this.offers.Add(offer);
                this.Market.AddOffer(offer);
            }
        }

        public void HandleOfferResults()
        {
            foreach (var offer in this.offers)
            {
                this.PriceBeliefs.Update(offer);
            }

            this.offers.Clear();
        }

    }
}
