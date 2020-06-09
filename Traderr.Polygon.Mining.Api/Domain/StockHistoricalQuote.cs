using System;
using Traderr.Polygon.Mining.Api.Core.Base;

namespace Traderr.Polygon.Mining.Api.Domain
{
    public class StockHistoricalQuote:Entity
    {
        public string Ticker { get; set; }

        public DateTime? SipDateTime { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? TrfDateTime { get; set; }

        public int? SequenceNumber { get; set; }

        public decimal? BidPrice { get; set; }

        public int? BidExchange { get; set; }

        public long? BidSize { get; set; }

        public decimal? AskPrice { get; set; }

        public int? AskExchange { get; set; }

        public long? AskSize { get; set; }

        public int? Tape { get; set; }
        public override string CollectionName { get; } = "StockHistoricalQuotes";
    }
}