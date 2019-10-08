using MongoDB.Driver;
using Trader.Infrastructure.DbContexts;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;

namespace Trader.Polygon.Api.Infrastructure.DbContexts
{
    public class PolygonDbContext : BaseDbContext, IPolygonDbContext
    {
        private readonly IMongoDatabase _database;

        public PolygonDbContext(IMongoDatabase database) 
            : base(database, null)
        
        {
            _database = database;
        }

       
    }
}