using System.Collections.Generic;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trader.Polygon.Api.Common.Configuration;
using Trader.Polygon.Api.Infrastructure.Interfaces;
using Trader.Polygon.Api.MessageHandlers.ServerSpecific.Interfaces;
using Trader.Polygon.Api.Messages;
using Trader.Polygon.Api.Pipelines;
using Trader.Polygon.Api.StreamMessages;
using Trader.Polygon.Core.Common.Enums;
using Trader.Polygon.Core.Streaming.Messages;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.MessageHandlers.ServerSpecific
{
    public class StockServerMessageHandler : IServerMessageHandler
    {
        private readonly ILogger<StockServerMessageHandler> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IStreamMessagePipelineProcessorResolver _streamMessagePipelineProcessorResolver;
        private readonly IMessageHub _messageHub;


        public StockServerMessageHandler(ILogger<StockServerMessageHandler> logger,
            IOptions<AppSettings> appSettings,
            IStreamMessagePipelineProcessorResolver streamMessagePipelineProcessorResolver,
            IMessageHub messageHub)
        {
            _logger = logger;
            _appSettings = appSettings;
            _streamMessagePipelineProcessorResolver = streamMessagePipelineProcessorResolver;
            _messageHub = messageHub;
        }

        public void Handle(IList<StreamingMessage> streamingMessages)
        {
            foreach (var message in streamingMessages)
            {
                switch (message.MessageType)
                {
                    case StreamingMessageType.Status:
                        HandleStatusMessage(message as StreamingStatusMessage);
                        break;
                    case StreamingMessageType.StockLastTrade:
                        HandleLastTradeMessage(message as StockLastTradeMessage);
                        break;
                    case StreamingMessageType.StockLastQuote:
                        HandleLastQuoteMessage(message as StockLastQuoteMessage);
                        break;
                    case StreamingMessageType.StockSecondAggregated:
                        HandleAggregatedMessage<StockSecondAggregatedMessage>(message as StockSecondAggregatedMessage);
                        break;
                    case StreamingMessageType.StockMinuteAggregated:
                        HandleAggregatedMessage<StockMinuteAggregatedMessage>(message as StockMinuteAggregatedMessage);
                        break;
                }
            }
        }

        private void HandleAggregatedMessage<T>(BaseStockAggregatedMessage message)
            where T : StreamingMessage
        {
            if (string.IsNullOrWhiteSpace(message?.Ticker) || message.Close == 0
                                                           || message.Open > 100000
                                                           || message.Open == 0
                                                           || message.High == 0
                                                           || message.Low == 0)
            {
                return;
            }

            var pipelineProcessor = _streamMessagePipelineProcessorResolver
                .Resolve<T>(typeof(StreamMessagePipeline<>));

            pipelineProcessor.Process(message as T);
        }

        private void HandleLastQuoteMessage(StockLastQuoteMessage message)
        {
            if (string.IsNullOrWhiteSpace(message?.Ticker) || message.Ask == 0
                                                           || message.AskSize == 0
                                                           || message.Bid == 0
                                                           || message.BidSize == 0)
            {
                return;
            }

            if (message.Ask < _appSettings.Value.DataProcessing.MinimumPrice &&
                message.Ask > _appSettings.Value.DataProcessing.MaximumPrice)
            {
                return;
            }

            if (message.Bid < _appSettings.Value.DataProcessing.MinimumPrice &&
                message.Bid > _appSettings.Value.DataProcessing.MaximumPrice)
            {
                return;
            }

            var pipelineProcessor = _streamMessagePipelineProcessorResolver
                .Resolve<StockLastQuoteMessage>(typeof(StreamMessagePipeline<>));
            pipelineProcessor.Process(message);
        }

        private void HandleStatusMessage(StreamingStatusMessage message)
        {
            _logger.LogInformation($"Stocks: {message.Status} - ${message.Message}");

            if (message.Message == "authenticated")
            {
                _messageHub.Publish(new StreamReceiverAuthenticatedMessage
                {
                    ServerType = DataServerType.Stocks
                });
            }

            if (message.Status == "connected")
            {
                _messageHub.Publish(new PolygonConnectedMessage());
            }
        }

        private void HandleLastTradeMessage(StockLastTradeMessage message)
        {
            if (string.IsNullOrWhiteSpace(message?.Ticker) ||
                message.Price == 0 || message.Size == 0 || message.Exchange == 0)
            {
                return;
            }

            if (message.Price < _appSettings.Value.DataProcessing.MinimumPrice &&
                message.Price > _appSettings.Value.DataProcessing.MaximumPrice)
            {
                return;
            }

            var pipelineProcessor = _streamMessagePipelineProcessorResolver
                .Resolve<StockLastTradeMessage>(typeof(StreamMessagePipeline<>));

            pipelineProcessor.Process(message);
        }
    }
}