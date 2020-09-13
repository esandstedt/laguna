using System;
using System.Collections.Generic;
using System.Text;

namespace Bazaar.Exchange
{
    public struct MarketHistory
    {
        public string Commodity { get; set; }
        public int SuccessfulTrades { get; set; }
        public double MoneyTraded { get; set; }
        public double AmountTraded { get; set; }
        public double AmountToBuy { get; set; }
        public double AmountToSell { get; set; }
        public double AveragePrice { get; set; }
        public double LowestSellingPrice { get; set; }
        public double HighestSellingPrice { get; set; }
        public double LowestBuyingPrice { get; set; }
        public double HighestBuyingPrice { get; set; }
    }
}
