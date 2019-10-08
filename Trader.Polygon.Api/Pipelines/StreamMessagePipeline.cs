using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Trader.Polygon.Api.DataSavers.Interfaces;
using Trader.Polygon.Api.Hubs;
using Trader.Polygon.Api.Logger;
using Trader.Polygon.Api.Pipelines.Interfaces;
using Trader.Polygon.Api.Services.Interfaces;
using Trader.Polygon.Core.Streaming.Messages;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.Pipelines
{
    public class StreamMessagePipeline<T> :IDisposable, IStreamMessagePipelineProcessor<T> where T : StreamingMessage
    {
        private readonly IPolygonDataSaver _dataSaver;
        private readonly ILogger<IPipelineProcessorLoggerCategory> _logger;
        private readonly StockPricesHub _stockPricesHub;
        private readonly ILastStockPriceMemoryHolder _lastStockPriceMemoryHolder;
        private readonly string _messageType;
        
        private readonly BroadcastBlock<T> _source;

        public StreamMessagePipeline(IPolygonDataSaver dataSaver,
            ILogger<IPipelineProcessorLoggerCategory> logger, 
            StockPricesHub stockPricesHub,
            ILastStockPriceMemoryHolder lastStockPriceMemoryHolder)
        {
            _dataSaver = dataSaver;
            _logger = logger;
            _stockPricesHub = stockPricesHub;
            _lastStockPriceMemoryHolder = lastStockPriceMemoryHolder;
            _messageType = typeof(T).Name;

            _source = new BroadcastBlock<T>(x => x);

            SetupSaveBatchPipeline();
            SetupPublishBatchPipeline();
        }

        private void SetupSaveBatchPipeline()
        {
            var actionBlockSave = new ActionBlock<IList<T>>(SaveBatch); 
            
            var saveTimerBatchBlock = TimerBatchBlock.Create<T>(50000, TimeSpan.FromSeconds(10));
            _source.LinkTo(saveTimerBatchBlock);

            saveTimerBatchBlock.LinkTo(actionBlockSave);
        }
        
        private void SaveBatch(IList<T> messages)
        {
            _logger.LogInformation($"{_messageType}: save on {messages.Count} records");
            _dataSaver.SaveAsync(messages).Wait();
        }
        
        private void PublishBatch(IList<T> messages)
        {
            var res = messages.GroupBy(d => d.Ticker)
                .Select(d => d.OrderByDescending(s => s.DateTime).FirstOrDefault())
                .ToList();

            if (messages is IList<StockLastQuoteMessage>)
            {
                var quoteMessages = res as List<StockLastQuoteMessage>;
                _stockPricesHub.SendQuotes(quoteMessages);
                _lastStockPriceMemoryHolder.AddOrUpdate(quoteMessages);
            }
            
            if (messages is IList<StockLastTradeMessage>)
            {
                var lastTradeMessages = res as List<StockLastTradeMessage>;
                _stockPricesHub.SendLastTrades(lastTradeMessages);
                _lastStockPriceMemoryHolder.AddOrUpdate(lastTradeMessages);
            }
            
            if (messages is IList<StockSecondAggregatedMessage>)
            {
                var aggMessages = res as List<StockSecondAggregatedMessage>;
                _stockPricesHub.SendAggregated(aggMessages);
                _lastStockPriceMemoryHolder.AddOrUpdate(aggMessages);
            }
           
        }

        private void SetupPublishBatchPipeline()
        {
            var actionBlockPublic = new ActionBlock<IList<T>>(PublishBatch); 
            
            var publishTimerBatchBlock = TimerBatchBlock.Create<T>(Int32.MaxValue, TimeSpan.FromSeconds(1));
            _source.LinkTo(publishTimerBatchBlock);

            publishTimerBatchBlock.LinkTo(actionBlockPublic);
        }

        public async Task Process(T record)
        {
            await _source.SendAsync(record);
        }
        
        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _source.Complete();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}