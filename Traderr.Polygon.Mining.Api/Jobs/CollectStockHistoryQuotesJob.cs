using System.Threading.Tasks;
using Traderr.Polygon.Mining.Api.Jobs.Interfaces;
using Traderr.Polygon.Mining.Api.Services.Interfaces;

namespace Traderr.Polygon.Mining.Api.Jobs
{
    public class CollectStockHistoryQuotesJob : ICollectStockHistoryQuotesJob
    {
        private readonly IStockHistoricalQuoteService _stockHistoricalQuoteService;

        public CollectStockHistoryQuotesJob(IStockHistoricalQuoteService stockHistoricalQuoteService)
        {
            _stockHistoricalQuoteService = stockHistoricalQuoteService;
        }

        public async Task Collect(HistoricalTicker historicalTicker)
        {
            await _stockHistoricalQuoteService.FetchAndSaveHistoricalQuotes(historicalTicker);
        }
    }
}