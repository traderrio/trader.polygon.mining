using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Flurl.Http;
using Trader.Infrastructure;
using Trader.Polygon.Api.Common.Configuration;
using Trader.Polygon.Api.DataSavers.Interfaces;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;

namespace Trader.Polygon.Api.DataSavers
{
    public class MongoPolygonDataSaver : ISpecificPolygonDataSaver
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IHostingEnvironment _env;
        private readonly IMongoCollection<StockLastTrade> _polygonStockLastTradesCollection;
        private readonly IMongoCollection<StockQuote> _polygonStockLastQuotesCollection;
        private readonly IMongoCollection<StockMinuteAggregated> _polygonStockMinuteAggregatedCollection;
        private readonly IMongoCollection<StockSecondAggregated> _polygonStockSecondAggregatedCollection;


        private readonly IMongoCollection<StockLastTrade> _apolloStockLastTradesCollection;
        private readonly IMongoCollection<StockQuote> _apolloStockLastQuotesCollection;
        private readonly IMongoCollection<StockMinuteAggregated> _apolloStockMinuteAggregatedCollection;
        private readonly IMongoCollection<StockSecondAggregated> _apolloStockSecondAggregatedCollection;

        public MongoPolygonDataSaver(IPolygonDbContext polygonDbContext,
            IApolloDbContext apolloDbContext, IHostingEnvironment env, IOptions<AppSettings> appSettings)
        {
            _env = env;
            _appSettings = appSettings;
            _polygonStockLastTradesCollection = polygonDbContext.GetCollection<StockLastTrade>();
            _polygonStockLastQuotesCollection = polygonDbContext.GetCollection<StockQuote>();
            _polygonStockMinuteAggregatedCollection = polygonDbContext.GetCollection<StockMinuteAggregated>();
            _polygonStockSecondAggregatedCollection = polygonDbContext.GetCollection<StockSecondAggregated>();

            _apolloStockLastTradesCollection = apolloDbContext.GetCollection<StockLastTrade>();
            _apolloStockLastQuotesCollection = apolloDbContext.GetCollection<StockQuote>();
            _apolloStockMinuteAggregatedCollection = apolloDbContext.GetCollection<StockMinuteAggregated>();
            _apolloStockSecondAggregatedCollection = apolloDbContext.GetCollection<StockSecondAggregated>();
        }

        public async Task SaveBulkLastTradeDataAsync(IList<StockLastTrade> stockLastTrades)
        {
            await _polygonStockLastTradesCollection.InsertManyAsync(stockLastTrades);
        }

        public async Task SaveBulkLastQuoteDataAsync(IList<StockQuote> quotes)
        {
            await _polygonStockLastQuotesCollection.InsertManyAsync(quotes);
        }

        public async Task SaveBulkMinuteAggregatedDataAsync(IList<StockMinuteAggregated> aggregated)
        {
            await _polygonStockMinuteAggregatedCollection.InsertManyAsync(aggregated);
        }

        public async Task SaveBulkSecondAggregatedDataAsync(IList<StockSecondAggregated> aggregated)
        {
            await _polygonStockSecondAggregatedCollection.InsertManyAsync(aggregated);
        }
    }
}