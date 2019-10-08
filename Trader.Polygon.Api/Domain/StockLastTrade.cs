using System;
using Trader.Domain;

namespace Trader.Polygon.Api.Domain
{
    public class StockLastTrade : Entity
    {
        public string Ticker { get; set; }
        public decimal Price { get; set; }
        public long Size { get; set; }
        public DateTime DateTime { get; set; }
        public int Exchange { get; set; }
        
        public override string CollectionName => "StockLastTrades";
    }
}