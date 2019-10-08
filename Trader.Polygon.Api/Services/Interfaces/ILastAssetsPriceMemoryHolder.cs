using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Polygon.Api.Services.Models;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.Services.Interfaces
{
    public interface ILastStockPriceMemoryHolder
    {
        void AddOrUpdate(IList<StockLastTradeMessage> message);
        void AddOrUpdate(IList<StockLastQuoteMessage> messages);
        void AddOrUpdate(IList<StockSecondAggregatedMessage> messages);
        Task LoadRecentAsync();
        Task<IList<AllStockPriceMessages>> FindMessages(IList<string> tickers);
    }
}