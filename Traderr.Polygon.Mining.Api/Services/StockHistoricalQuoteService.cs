using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver.Linq;
using Traderr.Next.Shared.Extensions;
using Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces;
using Traderr.Polygon.Mining.Api.DataAccess.Interfaces;
using Traderr.Polygon.Mining.Api.Domain;
using Traderr.Polygon.Mining.Api.Jobs;
using Traderr.Polygon.Mining.Api.Polygon;
using Traderr.Polygon.Mining.Api.Services.Interfaces;
using PolygonHistoricalNbboQuoteModel = Traderr.Polygon.Mining.Api.Polygon.Models.PolygonHistoricalNbboQuoteModel;

namespace Traderr.Polygon.Mining.Api.Services
{
    public class StockHistoricalQuoteService : IStockHistoricalQuoteService
    {
        private readonly IPolygonMiningDbContext _dataMiningDbContext;
        private readonly IPolygonApiClient _polygonApiClient;
        private readonly IMapper _mapper;

        public StockHistoricalQuoteService(IPolygonMiningDbContext dataMiningDbContext,
             IPolygonApiClient polygonApiClient,
            IMapper mapper)
        {
            _dataMiningDbContext = dataMiningDbContext;
            _polygonApiClient = polygonApiClient;
            _mapper = mapper;
        }

        public async Task FetchAndSaveHistoricalQuotes(HistoricalTicker historicalTicker)
        {
            var collection = GetDynamicNameCollection(historicalTicker.Ticker);
            _dataMiningDbContext.CreateHistoricalQuoteIndex(collection);

            var firstRecord = await collection.AsQueryable().Where(q => q.Ticker == historicalTicker.Ticker)
                .OrderBy(s => s.DateTime)
                .Take(1)
                .FirstOrDefaultAsync();

            if (firstRecord == null)
            {
                await DownloadAsync(historicalTicker.Ticker, historicalTicker.StartDate, DateTime.UtcNow.Date);
            }
            else if (historicalTicker.StartDate.Date < firstRecord.DateTime.Date)
            {
                await DownloadAsync(historicalTicker.Ticker, historicalTicker.StartDate,
                    firstRecord.DateTime.AddDays(-1));
            }

            var lastRecord = await collection.AsQueryable().Where(q => q.Ticker == historicalTicker.Ticker)
                .OrderByDescending(s => s.DateTime)
                .Take(1).FirstOrDefaultAsync();

            if (lastRecord != null && lastRecord.DateTime.Date < DateTime.UtcNow.Date)
            {
                var offsetTimestamp = lastRecord.DateTime.GetUnixTimeAsNanoSeconds(); 
                await DownloadAsync(historicalTicker.Ticker, lastRecord.DateTime.Date, DateTime.UtcNow.Date, offsetTimestamp);
            }
        }

        private Task DownloadAsync(string ticker, DateTime fromDate, DateTime toDate, long offsetTimestamp = 0)
        {
            return _polygonApiClient.HistoricalQuotesRange(ticker,
                fromDate, toDate, Handler, offsetTimestamp, 10000);
        }

        private async Task Handler(string ticker, IList<PolygonHistoricalNbboQuoteModel> quotes)
        {
            if (quotes.Count != 0)
            {
                var quoteEntities = _mapper.Map<IList<StockHistoricalQuote>>(quotes);
                var collection = GetDynamicNameCollection(ticker);
                await collection.InsertAsync(quoteEntities);
            }
        }

        private IMongoTable<StockHistoricalQuote> GetDynamicNameCollection(string ticker)
        {
            return _dataMiningDbContext.GetDynamicNameCollection<StockHistoricalQuote>(ticker);
        }
    }
}