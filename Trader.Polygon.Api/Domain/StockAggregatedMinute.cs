using System;
using Trader.Domain;

namespace Trader.Polygon.Api.Domain
{
    public class StockMinuteAggregated:Entity
    {
        public string Ticker { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Average { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long Volume { get; set; }
        public long AccumulatedVolume { get; set; }
        public long Vwap { get; set; }
        public override string CollectionName { get; } = "StockMinuteAggregated";
    }
}