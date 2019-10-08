using System.Threading.Tasks;
using MongoDB.Driver;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Api.Infrastructure.DbContexts.Interfaces;
using Trader.Polygon.Api.Jobs.Interfaces;
using Trader.Polygon.Api.Services.Interfaces;

namespace Trader.Polygon.Api.Jobs
{
    public class AggregateClosePricesJob : IAggregateClosePricesJob
    {
        private readonly IStockPriceRetriever _retriever;
        private readonly IMongoCollection<StockLastInfo> _collection;

        public AggregateClosePricesJob(IStockPriceRetriever retriever,
            IPolygonDbContext polygonDbContext)
        {
            _retriever = retriever;
            _collection = polygonDbContext.GetCollection<StockLastInfo>();
        }

        public async Task Aggregate()
        {
            var data = await _retriever.GetRecentAggregateAsync();

            var updateOptions = new UpdateOptions
            {
                IsUpsert = true
            };
           
            foreach (var d in data)
            {
                var info = new StockLastInfo
                {
                    Close = d.Close,
                    Ticker = d.Ticker,
                    DateTime = d.Start
                };

                var updateDefinitions = Builders<StockLastInfo>.Update
                    .Set(i => i.DateTime, info.DateTime)
                    .Set(i => i.Close, info.Close);

                await _collection.UpdateOneAsync(i => i.Ticker == info.Ticker,
                    updateDefinitions, updateOptions );
            }
        }
    }
}