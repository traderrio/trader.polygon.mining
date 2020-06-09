using MongoDB.Driver;
using Traderr.Polygon.Mining.Api.Core.DataAccess;
using Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces;
using Traderr.Polygon.Mining.Api.DataAccess.Interfaces;
using Traderr.Polygon.Mining.Api.Domain;

namespace Traderr.Polygon.Mining.Api.DataAccess
{
    public class PolygonMiningDbContext : MongoContext, IPolygonMiningDbContext
    {
        public PolygonMiningDbContext(MongoConfiguration configuration) : base(configuration)
        {
        }
        
        public void CreateHistoricalQuoteIndex(IMongoTable<StockHistoricalQuote> collection)
        {
            var indexKeyDef = new IndexKeysDefinitionBuilder<StockHistoricalQuote>().Ascending(m => m.DateTime);
            var createIndexModel = new CreateIndexModel<StockHistoricalQuote>(indexKeyDef);
            collection.Content.Indexes.CreateOne(createIndexModel);
        }
    }
}