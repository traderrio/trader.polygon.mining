using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Domain;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.DataSavers.Interfaces
{
	public interface IPolygonDataSaver 
	{
		Task SaveAsync<T>(IList<T> records) where T:StreamingMessage;
        Task SaveStockLastTradesAsync(IList<StockLastTrade> records);
        Task SaveStockLastQuotesAsync(IList<StockQuote> records);
	}
}