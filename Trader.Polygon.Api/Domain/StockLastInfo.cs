using System;
using Trader.Domain;

namespace Trader.Polygon.Api.Domain
{
    public class StockLastInfo:Entity
    {
        public string Ticker { get; set; }
        public decimal Close { get; set; }

        public DateTime DateTime { get; set; }
        
        public override string CollectionName { get; } = "StockLastInfo";
    }
}