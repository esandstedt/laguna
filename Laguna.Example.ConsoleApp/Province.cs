using Laguna.Agent;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class Province
    {
        public List<Industry> Industries { get; set; }
        public IMarket Market { get; set; }
        public List<Person> Persons { get; set; }

        public Province()
        {
            this.Industries = new List<Industry>();
            this.Market = new MarketImpl();
            this.Persons = new List<Person>();
        }

        public void Step()
        {
            foreach (var person in this.Persons)
            {
                person.Step();
                this.ApplySpoilRate(person.Inventory);
            }

            foreach (var industry in this.Industries)
            {
                industry.Step();
                this.ApplySpoilRate(industry.Inventory);
            }

            this.StepMarket();

            this.TaxIndustries();
        }

        private void ApplySpoilRate(Inventory inventory)
        {
            foreach (var commodity in inventory.Keys.ToList())
            {
                if (commodity == Constants.Money)
                {
                    continue;
                }

                var spoilRate = Constants.Commodities[commodity].SpoilRate;
                inventory.Set(
                    commodity,
                    (1 - spoilRate) * inventory.Get(commodity)
                );
            }
        }

        private void StepMarket()
        {
            var agents = new List<IMarketAgent>()
                .Concat(this.Persons)
                .Concat(this.Industries);

            var agentOffersMap = new List<IMarketAgent>()
                .Concat(this.Persons)
                .Concat(this.Industries)
                .ToDictionary(
                    x => x,
                    x => x.CreateOffers().ToList()
                );

            var offers = agentOffersMap.Values
                .Cast<IEnumerable<Offer>>()
                .Aggregate((a, b) => a.Concat(b));
            foreach (var offer in offers)
            {
                this.Market.AddOffer(offer);
            }

            this.Market.ResolveOffers();

            foreach (var pair in agentOffersMap)
            {
                pair.Key.HandleOfferResults(pair.Value);
            }
        }

        private void TaxIndustries()
        {
            var tax = 0.0;
            foreach (var industry in this.Industries)
            {
                var ceiling = 15000;

                var money = industry.Inventory.Get(Constants.Money);
                if (ceiling < money)
                {
                    var ratio = Math.Min(1.0, (money - ceiling) / 10000.0);
                    var amount = ratio * (money - ceiling);

                    tax += amount;
                    industry.Inventory.Add(Constants.Money, -amount);
                }
            }

            var credit = tax / this.Persons.Count;
            foreach (var person in this.Persons)
            {
                person.Inventory.Add(Constants.Money, credit);
            }
        }

    }
}
