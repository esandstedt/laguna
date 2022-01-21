using NUnit.Framework;

namespace Laguna.Agent.Tests
{
    [TestFixture]
    public class CostBeliefsTests
    {
        private static string ALPHA = "alpha";
        private static string BRAVO = "bravo";
        private static string CHARLIE = "charlie";

        [Test]
        public void SCSP_Simple()
        {
            var priceBeliefs = new PriceBeliefs();
            priceBeliefs.Set(ALPHA, 1, 2);
            priceBeliefs.Set(BRAVO, 1, 1);

            var costBeliefs = new CostBeliefs(priceBeliefs);
            costBeliefs.Begin();
            costBeliefs.Consume(ALPHA, 10);
            costBeliefs.Produce(BRAVO, 1);
            var result = costBeliefs.End().ToDictionary(x => x.Commodity);

            Assert.AreEqual(10, result[BRAVO].MinPrice);
            Assert.AreEqual(20, result[BRAVO].MaxPrice);
        }

        [Test]
        public void SCSP_Splits_Price()
        {
            var priceBeliefs = new PriceBeliefs();
            priceBeliefs.Set(ALPHA, 1, 2);
            priceBeliefs.Set(BRAVO, 1, 1);

            var costBeliefs = new CostBeliefs(priceBeliefs);
            costBeliefs.Begin();
            costBeliefs.Consume(ALPHA, 10);
            costBeliefs.Produce(BRAVO, 4);
            var result = costBeliefs.End().ToDictionary(x => x.Commodity);

            Assert.AreEqual(2.5, result[BRAVO].MinPrice);
            Assert.AreEqual(5, result[BRAVO].MaxPrice);
        }

        [Test]
        public void MCSP_Simple()
        {
            var priceBeliefs = new PriceBeliefs();
            priceBeliefs.Set(ALPHA, 1, 2);
            priceBeliefs.Set(BRAVO, 2, 3);
            priceBeliefs.Set(CHARLIE, 1, 1);

            var costBeliefs = new CostBeliefs(priceBeliefs);
            costBeliefs.Begin();
            costBeliefs.Consume(ALPHA, 10);
            costBeliefs.Consume(BRAVO, 20);
            costBeliefs.Produce(CHARLIE, 1);
            var result = costBeliefs.End().ToDictionary(x => x.Commodity);

            Assert.AreEqual(50, result[CHARLIE].MinPrice);
            Assert.AreEqual(80, result[CHARLIE].MaxPrice);
        }

        [Test]
        public void MCSP_Splits_Price()
        {
            var priceBeliefs = new PriceBeliefs();
            priceBeliefs.Set(ALPHA, 1, 2);
            priceBeliefs.Set(BRAVO, 2, 3);
            priceBeliefs.Set(CHARLIE, 1, 1);

            var costBeliefs = new CostBeliefs(priceBeliefs);
            costBeliefs.Begin();
            costBeliefs.Consume(ALPHA, 10);
            costBeliefs.Consume(BRAVO, 20);
            costBeliefs.Produce(CHARLIE, 5);
            var result = costBeliefs.End().ToDictionary(x => x.Commodity);

            Assert.AreEqual(10, result[CHARLIE].MinPrice);
            Assert.AreEqual(16, result[CHARLIE].MaxPrice);
        }

        [Test]
        public void SCMP_Simple()
        {
            var priceBeliefs = new PriceBeliefs();
            priceBeliefs.Set(ALPHA, 1, 2);
            priceBeliefs.Set(BRAVO, 1, 1);
            priceBeliefs.Set(CHARLIE, 1, 1);

            var costBeliefs = new CostBeliefs(priceBeliefs);
            costBeliefs.Begin();
            costBeliefs.Consume(ALPHA, 10);
            costBeliefs.Produce(BRAVO, 2);
            costBeliefs.Produce(CHARLIE, 3);
            var result = costBeliefs.End().ToDictionary(x => x.Commodity);

            Assert.AreEqual(2, result[BRAVO].MinPrice);
            Assert.AreEqual(4, result[BRAVO].MaxPrice);

            Assert.AreEqual(2, result[CHARLIE].MinPrice);
            Assert.AreEqual(4, result[CHARLIE].MaxPrice);
        }

        [Test]
        public void SCMP_Uses_PriceBeliefs_To_Balance_Price()
        {
            var priceBeliefs = new PriceBeliefs();
            priceBeliefs.Set(ALPHA, 1, 4);
            priceBeliefs.Set(BRAVO, 1, 6);
            priceBeliefs.Set(CHARLIE, 2, 4);

            var costBeliefs = new CostBeliefs(priceBeliefs);
            costBeliefs.Begin();
            costBeliefs.Consume(ALPHA, 8);
            costBeliefs.Produce(BRAVO, 2);
            costBeliefs.Produce(CHARLIE, 3);
            var result = costBeliefs.End().ToDictionary(x => x.Commodity);

            Assert.AreEqual(1, result[BRAVO].MinPrice);
            Assert.AreEqual(8, result[BRAVO].MaxPrice);

            Assert.AreEqual(2, result[CHARLIE].MinPrice);
            Assert.AreEqual(5.33, result[CHARLIE].MaxPrice, 0.01);
        }

    }
}