using System.Threading.Tasks;
using Traderr.Polygon.Mining.Api.Jobs;

namespace Traderr.Polygon.Mining.Api.Services.Interfaces
{
    public interface IStockHistoricalQuoteService
    {
        Task FetchAndSaveHistoricalQuotes(HistoricalTicker historicalTicker);
    }
}