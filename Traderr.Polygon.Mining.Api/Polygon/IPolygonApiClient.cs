using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Traderr.Polygon.Mining.Api.Polygon.Models;

namespace Traderr.Polygon.Mining.Api.Polygon
{
    public interface IPolygonApiClient
    {
        Task<IList<PolygonHistoricalNbboQuoteModel>> GetHistoricalQuotesForDateAsync(
            string symbol, string date,  
            long offset = 0,
            int limit = 50000);

        Task HistoricalQuotesRange(string symbol, DateTime fromDate, DateTime toDate,
            Func<string, IList<PolygonHistoricalNbboQuoteModel>, Task> handler,
            long offset = 0,
            int limit = 50000);
    }
}