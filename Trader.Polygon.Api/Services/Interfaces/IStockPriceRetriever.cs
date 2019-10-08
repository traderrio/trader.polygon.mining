using System.Collections.Generic;
using System.Threading.Tasks;
using Trader.Polygon.Api.Domain;

namespace Trader.Polygon.Api.Services.Interfaces
{
    public interface IStockPriceRetriever
    {
        Task<IList<StockSecondAggregated>> GetRecentAggregateAsync();
    }
}