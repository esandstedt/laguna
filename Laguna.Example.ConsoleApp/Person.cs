using Laguna.Agent;
using Laguna.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laguna.Example.ConsoleApp
{
    public class Person : Agent
    {
        public CostBeliefs CostBeliefs { get; set; }

        public Person()
        {
            this.CostBeliefs = new CostBeliefs(this.PriceBeliefs);

            this.Inventory.Add(Constants.Money, 100);
        }

        public void Step()
        {
            this.CostBeliefs.Begin();
            this.CostBeliefs.Produce(Constants.UnskilledWork, 1);

            this.Inventory.Set(
                Constants.Food,
                Math.Max(0, this.Inventory.Get(Constants.Food) - 1)
            );
            this.CostBeliefs.Consume(Constants.Food, 1);

            if (0 < this.Inventory.Get(Constants.Timber))
            {
                this.Inventory.Set(
                    Constants.Timber,
                    Math.Max(0, this.Inventory.Get(Constants.Timber) - 1)
                );
                this.CostBeliefs.Consume(Constants.Timber, 1);
            }

            if (2 < this.Inventory.Get(Constants.Timber))
            {
                this.Inventory.Set(
                    Constants.Timber,
                    Math.Max(0, this.Inventory.Get(Constants.Food) - 1)
                );
                this.CostBeliefs.Consume(Constants.Food, 1);
            }

            this.CostBeliefs.End();
        }

        public override IEnumerable<Offer> CreateOffers()
        {
            yield return new Offer(
                OfferType.Sell,
                Constants.UnskilledWork,
                this.PriceBeliefs.GetRandom(Constants.UnskilledWork),
                1
            );


            var buyOffers = new List<Offer>();


            var x = Math.Max(3, 5 - this.Inventory.Get(Constants.Food));
            while (0 < x)
            {
                buyOffers.Add(new Offer(
                    OfferType.Buy,
                    Constants.Food,
                    this.PriceBeliefs.GetRandom(Constants.Food),
                    Math.Min(1, x)
                ));

                x -= 1;
            }

            if (2 < this.Inventory.Get(Constants.Food))
            {
                buyOffers.Add(new Offer(
                        OfferType.Buy,
                        Constants.Timber,
                        this.PriceBeliefs.GetRandom(Constants.Timber),
                        Math.Max(2, 5 - this.Inventory.Get(Constants.Timber))
                ));
            }

            var money = this.Inventory.Get(Constants.Money);
            foreach (var offer in buyOffers.SelectMany(x => x.Split(1)))
            {
                money -= offer.Price * offer.Amount;

                if (money < 0)
                {
                    break;
                }

                yield return offer;
            }
        }
    }
}
