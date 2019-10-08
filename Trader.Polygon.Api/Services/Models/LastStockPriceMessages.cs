using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.Services.Models
{
    public class AllStockPriceMessages
    {
        public string Ticker { get; set; }
        public StockLastTradeMessage LastTrade { get; set; }
        public StockLastQuoteMessage Quote { get; set; }
        public StockSecondAggregatedMessage Aggregated { get; set; }
    }
}