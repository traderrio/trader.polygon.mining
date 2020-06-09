using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Traderr.Polygon.Mining.Api.Core.Infrastructure;

namespace Traderr.Polygon.Mining.Api.Configurations
{
    internal static class HangfireConfiguration
    {
        public static void AddAppHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireConnectionString = configuration.GetConnectionString("Hangfire");
            GlobalJobFilters.Filters.Add(new SkipWhenPreviousJobIsRunningAttribute());
            services.AddHangfire(conf =>
            {
                var storageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new DropMongoMigrationStrategy(),
                    }
                };
                conf.UseMongoStorage(hangfireConnectionString, storageOptions);
            });
        }

        public static void UseAppHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(), }
            });
        }
    }
}