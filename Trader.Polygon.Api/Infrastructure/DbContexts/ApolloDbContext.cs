using MongoDB.Driver;
using Trader.Infrastructure.DbContexts;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;

namespace Trader.Polygon.Api.Infrastructure.DbContexts
{
    public class ApolloDbContext : BaseDbContext, IApolloDbContext
    {
        private readonly IMongoDatabase _database;

        public ApolloDbContext(IMongoDatabase database) 
            : base(database, null)
        
        {
            _database = database;
        }

       
    }
}