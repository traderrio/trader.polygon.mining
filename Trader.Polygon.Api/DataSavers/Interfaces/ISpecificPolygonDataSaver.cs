using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Polygon.Api.Domain;

namespace Trader.Polygon.Api.DataSavers.Interfaces
{
	public interface ISpecificPolygonDataSaver
	{
		Task SaveBulkLastTradeDataAsync(IList<StockLastTrade> stockLastTrades);
		Task SaveBulkLastQuoteDataAsync(IList<StockQuote> quotes);
		Task SaveBulkMinuteAggregatedDataAsync(IList<StockMinuteAggregated> aggregated);
		Task SaveBulkSecondAggregatedDataAsync(IList<StockSecondAggregated> aggregated);
	}
}