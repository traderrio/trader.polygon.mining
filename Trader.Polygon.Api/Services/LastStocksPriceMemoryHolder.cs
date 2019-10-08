using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trader.Polygon.Api.Services.Interfaces;
using Trader.Polygon.Api.Services.Models;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.Services
{
     public class LastStocksPriceMemoryHolder : ILastStockPriceMemoryHolder
    {
        private readonly IStockPriceInfoRetriever _priceInfoRetriever;
        private readonly ConcurrentDictionary<string, AllStockPriceMessages> _assetsPrices;

        public LastStocksPriceMemoryHolder(IStockPriceInfoRetriever priceInfoRetriever)
        {
            _priceInfoRetriever = priceInfoRetriever;
            _assetsPrices = new ConcurrentDictionary<string, AllStockPriceMessages>();
        }
        
        public async Task LoadRecentAsync()
        {
            var lastInfo = await _priceInfoRetriever.GetLastInfo();
            foreach (var info in lastInfo)
            {
                _assetsPrices.TryAdd(info.Ticker, new AllStockPriceMessages
                {
                    Aggregated = new StockSecondAggregatedMessage
                    {
                        Close = info.Close,
                        Ticker = info.Ticker,
                        DateTime = info.DateTime
                    },
                    Ticker = info.Ticker
                });
            }
        }

        public async Task<IList<AllStockPriceMessages>> FindMessages(IList<string> tickers)
        {
           var stocksPrices = _assetsPrices.Where(a => tickers.Contains(a.Key))
               .Select(a => a.Value)
               .ToList();

           return await Task.FromResult(stocksPrices);
        }

        public void AddOrUpdate(IList<StockLastTradeMessage> messages)
        {
            foreach (var message in messages)
            {
                if (_assetsPrices.ContainsKey(message.Ticker))
                {
                    _assetsPrices[message.Ticker].LastTrade = message;
                }
                else
                {
                    _assetsPrices.TryAdd(message.Ticker, new AllStockPriceMessages
                    {
                        LastTrade = message
                    });
                }
            }
           
        }
        
        public void AddOrUpdate(IList<StockLastQuoteMessage> messages)
        {
            foreach (var message in messages)
            {
                if (_assetsPrices.ContainsKey(message.Ticker))
                {
                    _assetsPrices[message.Ticker].Quote = message;
                }
                else
                {
                    _assetsPrices.TryAdd(message.Ticker, new AllStockPriceMessages
                    {
                        Quote = message
                    });
                }
            }
          
        }
        
        public void AddOrUpdate(IList<StockSecondAggregatedMessage> messages)
        {
            foreach (var message in messages)
            {
                if (_assetsPrices.ContainsKey(message.Ticker))
                {
                    _assetsPrices[message.Ticker].Aggregated = message;
                }
                else
                {
                    _assetsPrices.TryAdd(message.Ticker, new AllStockPriceMessages
                    {
                        Aggregated = message
                    });
                }
            }
            
        }
    }
}