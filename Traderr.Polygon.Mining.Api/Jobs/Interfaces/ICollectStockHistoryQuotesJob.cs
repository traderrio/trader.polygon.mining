using System.Threading.Tasks;

namespace Traderr.Polygon.Mining.Api.Jobs.Interfaces
{
    public interface ICollectStockHistoryQuotesJob
    {
        Task Collect(HistoricalTicker historicalTicker);
    }
}