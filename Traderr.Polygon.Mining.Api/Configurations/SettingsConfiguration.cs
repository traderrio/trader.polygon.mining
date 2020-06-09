using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Traderr.Polygon.Mining.Api.Polygon;
using Traderr.Polygon.Mining.Api.Services;
using Traderr.Polygon.Mining.Api.Services.Interfaces;
using Traderr.Polygon.Mining.Api.Settings;

namespace Traderr.Polygon.Mining.Api.Configurations
{
    internal static class SettingsConfiguration
    {
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
        }
    }
}