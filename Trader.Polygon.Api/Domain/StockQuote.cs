using System;
using Trader.Domain;

namespace Trader.Polygon.Api.Domain
{
    public class StockQuote:Entity
    {
        public string Ticker { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public DateTime DateTime { get; set; }
        public long AskSize { get; set; }
        public long BidSize { get; set; }
        public int AskExchange { get; set; }
        public int BidExchange { get; set; }

        public override string CollectionName { get; } = "StockQuotes";
    }
}