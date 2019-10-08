using AutoMapper;
using Easy.MessageHub;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Trader.Infrastructure.Hangfire;
using Trader.Polygon.Api.Common.Configuration;
using Trader.Polygon.Api.DataSavers;
using Trader.Polygon.Api.DataSavers.Interfaces;
using Trader.Polygon.Api.Hubs;
using Trader.Polygon.Api.Infrastructure;
using Trader.Polygon.Api.Infrastructure.DbContexts;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;
using Trader.Polygon.Api.Infrastructure.Interfaces;
using Trader.Polygon.Api.Jobs;
using Trader.Polygon.Api.Jobs.Interfaces;
using Trader.Polygon.Api.MessageHandlers;
using Trader.Polygon.Api.MessageHandlers.Interfaces;
using Trader.Polygon.Api.MessageHandlers.ServerSpecific;
using Trader.Polygon.Api.Pipelines;
using Trader.Polygon.Api.Receivers;
using Trader.Polygon.Api.Receivers.Interfaces;
using Trader.Polygon.Api.Services;
using Trader.Polygon.Api.Services.Interfaces;
using Trader.Services;
using Trader.Services.Interfaces;
using ITradingActivityService = Trader.Polygon.Api.Services.ITradingActivityService;
using TradingActivityService = Trader.Polygon.Api.Services.TradingActivityService;

namespace Trader.Polygon.Api.Config
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomisedSignalR(this IServiceCollection services)
        {
            services
                .AddSignalR()
                .AddMessagePackProtocol();

            services.AddSingleton<StockPricesHub>();

            return services;
        }

        public static IServiceCollection AddCustomisedAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            return services;
        }

        public static IServiceCollection AddDataLayers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPolygonDbContext>(opt =>
            {
                var polygonConnectionString = new MongoConnectionString(configuration.GetConnectionString("Polygon"));
                var client = new MongoClient(polygonConnectionString.Settings);
                var db = client.GetDatabase(polygonConnectionString.Database);
                return new PolygonDbContext(db);
            });

            services.AddSingleton<IApolloDbContext>(opt =>
            {
                var apolloConnectionString = new MongoConnectionString(configuration.GetConnectionString("Apollo"));
                var client = new MongoClient(apolloConnectionString.Settings);
                var db = client.GetDatabase(apolloConnectionString.Database);
                return new ApolloDbContext(db);
            });

            return services;
        }

        public static IServiceCollection AddCustomisedCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                        builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins("http://localhost:4200",
                                "https://qa.traderr.io",
                                "https://traderr.io",
                                "https://qa-api.traderr.io",
                                "https://api.traderr.io")
                            .AllowCredentials()
                );
            });

            return services;
        }

        public static IServiceCollection AddCustomisedMvc(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            return services;
        }

        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
            services.AddSingleton(configuration);

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IPolygonDataSaver, PolygonDataSaver>();
            services.AddSingleton<MongoPolygonDataSaver>();
            services.AddSingleton<IStocksStreamReceiver, StocksStreamReceiver>();
            services.AddSingleton(typeof(StreamMessagePipeline<>));
            services.AddSingleton<IStreamMessageHandler, StreamMessageHandler>();
            services.AddSingleton<StockServerMessageHandler>();

            services.AddSingleton<IStreamMessagePipelineProcessorResolver, StreamMessagePipelineProcessorResolver>();

            services.AddScoped<ITradingActivityService, TradingActivityService>();

            services.AddSingleton<IMessageHub>(MessageHub.Instance);
            services.AddScoped<IStockPriceRetriever, StockPriceRetriever>();
            services.AddScoped<IStockPriceInfoRetriever, StockPriceInfoRetriever>();
            services.AddSingleton<ILastStockPriceMemoryHolder, LastStocksPriceMemoryHolder>();

            return services;
        }

        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            services.AddScoped(typeof(IJobRunner<>), typeof(JobRunner<>));
            services.AddScoped<IAggregateClosePricesJob, AggregateClosePricesJob>();
            services.AddSingleton<IHangfireJobFactory, HangfireJobFactory>();

            return services;
        }

      public static IServiceCollection AddCustomizedHangfire(this IServiceCollection services,
            IConfiguration configuration)
        {
 
            GlobalJobFilters.Filters.Add(new SkipWhenPreviousJobIsRunningAttribute());

            return services;
        }
    }
}