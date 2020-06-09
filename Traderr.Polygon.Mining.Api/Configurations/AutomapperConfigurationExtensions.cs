using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Traderr.Polygon.Mining.Api.Automapper;

namespace Traderr.Polygon.Mining.Api.Configurations
{
    internal static class AutomapperConfigurationExtensions
    {
        public static void AddAppAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MainProfile));
        }
    }
}
