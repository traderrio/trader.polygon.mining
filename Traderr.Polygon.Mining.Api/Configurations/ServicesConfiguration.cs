using Microsoft.Extensions.DependencyInjection;
using Traderr.Polygon.Mining.Api.Polygon;
using Traderr.Polygon.Mining.Api.Services;
using Traderr.Polygon.Mining.Api.Services.Interfaces;

namespace Traderr.Polygon.Mining.Api.Configurations
{
    internal static class ServicesConfiguration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPolygonApiClient, PolygonApiClient>();
            services.AddScoped<IStockHistoricalQuoteService, StockHistoricalQuoteService>();
        }
    }
}