using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trader.Polygon.Api.Common;
using Trader.Polygon.Api.Common.Configuration;
using Trader.Polygon.Api.DataSavers.Interfaces;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Core.Streaming.Messages;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.DataSavers
{
    public class PolygonDataSaver : IPolygonDataSaver
    {
        private readonly IOptions<AppSettings> _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private ISpecificPolygonDataSaver _specificPolygonDataSaver;

        public PolygonDataSaver(IOptions<AppSettings> options,
            IServiceProvider serviceProvider, IMapper mapper)
        {
            _options = options;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            ResolveSpecificDataSaver();
        }

        public async Task SaveAsync<T>(IList<T> records) where T : StreamingMessage
        {
            if (typeof(T) == typeof(StockLastTradeMessage))
            {
                var lastTrades = _mapper.Map<IList<StockLastTrade>>(records);
                await _specificPolygonDataSaver.SaveBulkLastTradeDataAsync(lastTrades);
            }
            else if (typeof(T) == typeof(StockLastQuoteMessage))
            {
                var lastQuotes = _mapper.Map<IList<StockQuote>>(records);
                await _specificPolygonDataSaver.SaveBulkLastQuoteDataAsync(lastQuotes);
            }
            else if (typeof(T) == typeof(StockSecondAggregatedMessage))
            {
                var lastQuotes = _mapper.Map<IList<StockSecondAggregated>>(records);
                await _specificPolygonDataSaver.SaveBulkSecondAggregatedDataAsync(lastQuotes);
            }
            else if (typeof(T) == typeof(StockMinuteAggregatedMessage))
            {
                var lastQuotes = _mapper.Map<IList<StockMinuteAggregated>>(records);
                await _specificPolygonDataSaver.SaveBulkMinuteAggregatedDataAsync(lastQuotes);
            }
        }

        public async Task SaveStockLastTradesAsync(IList<StockLastTrade> records)
        {
            await _specificPolygonDataSaver.SaveBulkLastTradeDataAsync(records);
        }

        public async Task SaveStockLastQuotesAsync(IList<StockQuote> records)
        {
            await _specificPolygonDataSaver.SaveBulkLastQuoteDataAsync(records);
        }

        private void ResolveSpecificDataSaver()
        {
            var saverType = _options.Value.Polygon.SaverType;
            switch (saverType)
            {
                case DataSaverType.Mongo:
                    _specificPolygonDataSaver = _serviceProvider.GetRequiredService<MongoPolygonDataSaver>();
                    break;
                default: throw new InvalidOperationException($"Saver type is not supported: {saverType}");
            }
        }
    }
}