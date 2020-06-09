using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs;
using Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs.Interfaces;
using Traderr.Polygon.Mining.Api.Jobs;
using Traderr.Polygon.Mining.Api.Jobs.Interfaces;

namespace Traderr.Polygon.Mining.Api.Configurations
{
    internal static class JobsConfiguration
    {
        public static void AddJobs(this IServiceCollection services)
        {
            services.AddScoped(typeof(IJobRunner<>), typeof(JobRunner<>));

            services.AddScoped<IJobFactory, JobFactory>();
            
            services.AddTransient<IHistoricalQuotesJobCreator, HistoricalQuotesJobCreator>();
            services.AddTransient<ICollectStockHistoryQuotesJob, CollectStockHistoryQuotesJob>();


        }

        public static void CreateJobs(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var jobFactory = scope.ServiceProvider.GetRequiredService<IJobFactory>();
            jobFactory.CreateCollectHistoryQuoteData();
        }
    }
}