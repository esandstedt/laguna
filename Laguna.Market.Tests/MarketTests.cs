using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laguna.Market.Tests
{
    [TestFixture]
    public class MarketTests
    {
        private static readonly string COMMODITY = "commodity";

        [Test]
        public void Simple()
        {
            var market = new MarketImpl();

            var buy = new Offer(OfferType.Buy, COMMODITY, 1, 1);
            var sell = new Offer(OfferType.Sell, COMMODITY, 1, 1);

            market.AddOffer(buy);
            market.AddOffer(sell);

            market.ResolveOffers();

            Assert.AreEqual(1, buy.Results.Count);
            var buyResult = buy.Results.Single();
            Assert.AreEqual(1, buyResult.Amount);
            Assert.AreEqual(1, buyResult.Price);

            Assert.AreEqual(1, sell.Results.Count);
            var sellResult = sell.Results.Single();
            Assert.AreEqual(1, sellResult.Amount);
            Assert.AreEqual(1, sellResult.Price);
        }

        [Test]
        public void Simple_Limited_By_Amount([Values("buy", "sell")] string limited)
        {
            var market = new MarketImpl();

            var buy = new Offer(OfferType.Buy, COMMODITY, 1, limited == "buy" ? 1 : 2);
            var sell = new Offer(OfferType.Sell, COMMODITY, 1, limited == "buy" ? 2 : 1);

            market.AddOffer(buy);
            market.AddOffer(sell);

            market.ResolveOffers();

            Assert.AreEqual(1, buy.Results.Count);
            var buyResult = buy.Results.Single();
            Assert.AreEqual(1, buyResult.Amount);
            Assert.AreEqual(1, buyResult.Price);

            Assert.AreEqual(1, sell.Results.Count);
            var sellResult = sell.Results.Single();
            Assert.AreEqual(1, sellResult.Amount);
            Assert.AreEqual(1, sellResult.Price);
        }

        [Test]
        public void Simple_Halfway_Price_If_Buy_Price_Is_Greater()
        {
            var market = new MarketImpl();

            var buy = new Offer(OfferType.Buy, COMMODITY, 3, 1);
            var sell = new Offer(OfferType.Sell, COMMODITY, 1, 1);

            market.AddOffer(buy);
            market.AddOffer(sell);

            market.ResolveOffers();

            Assert.AreEqual(1, buy.Results.Count);
            var buyResult = buy.Results.Single();
            Assert.AreEqual(1, buyResult.Amount);
            Assert.AreEqual(2, buyResult.Price);

            Assert.AreEqual(1, sell.Results.Count);
            var sellResult = sell.Results.Single();
            Assert.AreEqual(1, sellResult.Amount);
            Assert.AreEqual(2, sellResult.Price);
        }

        [Test]
        public void Simple_No_Trade_If_Buy_Price_Below_Sell()
        {
            var market = new MarketImpl();

            var buy = new Offer(OfferType.Buy, COMMODITY, 1, 1);
            var sell = new Offer(OfferType.Sell, COMMODITY, 2, 1);

            market.AddOffer(buy);
            market.AddOffer(sell);

            market.ResolveOffers();

            Assert.AreEqual(0, buy.Results.Count);
            Assert.AreEqual(0, sell.Results.Count);
        }

        [Test]
        public void Multiple_Buyers()
        {
            var market = new MarketImpl();

            var buy1 = new Offer(OfferType.Buy, COMMODITY, 1, 1);
            var buy2 = new Offer(OfferType.Buy, COMMODITY, 1, 1);
            var sell = new Offer(OfferType.Sell, COMMODITY, 1, 2);

            market.AddOffer(buy1);
            market.AddOffer(buy2);
            market.AddOffer(sell);

            market.ResolveOffers();

            Assert.AreEqual(1, buy1.Results.Count);
            var buy1Result = buy1.Results.Single();
            Assert.AreEqual(1, buy1Result.Amount);
            Assert.AreEqual(1, buy1Result.Price);

            Assert.AreEqual(1, buy2.Results.Count);
            var buy2Result = buy2.Results.Single();
            Assert.AreEqual(1, buy2Result.Amount);
            Assert.AreEqual(1, buy2Result.Price);

            Assert.AreEqual(2, sell.Results.Count);
            foreach (var result in sell.Results)
            {
                Assert.AreEqual(1, result.Amount);
                Assert.AreEqual(1, result.Price);
            }
        }

        [Test]
        public void Multiple_Sellers()
        {
            var market = new MarketImpl();

            var buy = new Offer(OfferType.Buy, COMMODITY, 1, 2);
            var sell1 = new Offer(OfferType.Sell, COMMODITY, 1, 1);
            var sell2 = new Offer(OfferType.Sell, COMMODITY, 1, 1);

            market.AddOffer(buy);
            market.AddOffer(sell1);
            market.AddOffer(sell2);

            market.ResolveOffers();

            Assert.AreEqual(2, buy.Results.Count);
            foreach (var result in buy.Results)
            {
                Assert.AreEqual(1, result.Amount);
                Assert.AreEqual(1, result.Price);
            }

            Assert.AreEqual(1, sell1.Results.Count);
            var sell1Result = sell1.Results.Single();
            Assert.AreEqual(1, sell1Result.Amount);
            Assert.AreEqual(1, sell1Result.Price);

            Assert.AreEqual(1, sell2.Results.Count);
            var sell2Result = sell2.Results.Single();
            Assert.AreEqual(1, sell2Result.Amount);
            Assert.AreEqual(1, sell2Result.Price);
        }
    }
}
