using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Traderr.Polygon.Mining.Api.Core.DataAccess;
using Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces;
using Traderr.Polygon.Mining.Api.DataAccess;
using Traderr.Polygon.Mining.Api.DataAccess.Interfaces;

namespace Traderr.Polygon.Mining.Api.Configurations
{
    internal static class DataContextConfiguration
    {
        public static void AddDataContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IMongoTable<>), typeof(MongoTable<>));
            services.AddSingleton<IPolygonMiningDbContext>(opt =>
            {
                var mongoConfiguration = new MongoConfiguration(configuration.GetConnectionString("Mongo"));
                return new PolygonMiningDbContext(mongoConfiguration);
            });
        }
    }
}