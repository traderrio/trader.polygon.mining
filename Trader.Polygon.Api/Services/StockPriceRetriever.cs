using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;
using Trader.Polygon.Api.Services.Interfaces;

namespace Trader.Polygon.Api.Services
{
    public class StockPriceRetriever :  IStockPriceRetriever
    {
        private readonly IMongoCollection<StockSecondAggregated> _secondAggregatedCollection;

        public StockPriceRetriever(IPolygonDbContext polygonDbContext)
        {
            _secondAggregatedCollection = polygonDbContext.GetCollection<StockSecondAggregated>();
        }

        public async Task<IList<StockSecondAggregated>> GetRecentAggregateAsync()
        {
            var lastQuotes = await _secondAggregatedCollection.Aggregate()
                .SortByDescending(t => t.Start)
                .Group(trade => trade.Ticker,  g => new StockSecondAggregated
                {
                    Ticker = g.First().Ticker,
                    Start = g.First().Start,
                    End = g.First().End,
                    Average = g.First().Average,
                    Close = g.First().Close,
                    High = g.First().High,
                    Low = g.First().Low,
                    Open = g.First().Open,
                    Volume = g.First().Volume,
                    Vwap = g.First().Vwap,
                    AccumulatedVolume = g.First().AccumulatedVolume,
                }) 
                .ToListAsync();

            return lastQuotes;
        }
    }
}