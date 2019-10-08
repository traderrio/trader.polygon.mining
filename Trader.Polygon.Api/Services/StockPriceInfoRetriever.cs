using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;
using Trader.Polygon.Api.Services.Interfaces;

namespace Trader.Polygon.Api.Services
{
    public class StockPriceInfoRetriever : IStockPriceInfoRetriever
    {
        private readonly IMongoCollection<StockLastInfo> _lastInfoCollection;

        public StockPriceInfoRetriever(IPolygonDbContext polygonDbContext)
        {
            _lastInfoCollection = polygonDbContext.GetCollection<StockLastInfo>();
        }

        public async Task<IList<StockLastInfo>> GetLastInfo()
        {
            var lastInfo = await _lastInfoCollection.Find(_ => true).ToListAsync();
            return lastInfo;
        }
    }
}