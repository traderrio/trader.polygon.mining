using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easy.MessageHub;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Trader.Polygon.Api.Common.Configuration;
using Trader.Polygon.Api.Converters;
using Trader.Polygon.Api.MessageHandlers.Interfaces;
using Trader.Polygon.Api.Messages;
using Trader.Polygon.Api.Receivers.Interfaces;
using Trader.Polygon.Core.Common.Enums;
using Trader.Polygon.Core.Streaming.Messages;
using Websocket.Client;

namespace Trader.Polygon.Api.Receivers
{
    public class StocksStreamReceiver : IStocksStreamReceiver, IDisposable
    {
        private readonly ILogger<IStocksStreamReceiver> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IStreamMessageHandler _streamMessageHandler;
        private readonly IHostingEnvironment _hostingEnvironment;
        private WebsocketClient _client;
        private IDisposable _disconnectionHappenedSubs;
        private IDisposable _reconnectionHappenedSubs;
        private IDisposable _messageReceivedSubs;

        public StocksStreamReceiver(ILogger<IStocksStreamReceiver> logger,
            IOptions<AppSettings> appSettings,
            IStreamMessageHandler streamMessageHandler,
            IMessageHub messageHub,
            IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _appSettings = appSettings;
            _streamMessageHandler = streamMessageHandler;
            _hostingEnvironment = hostingEnvironment;


            messageHub.Subscribe<StreamReceiverAuthenticatedMessage>(SubscribeToStreams);
            messageHub.Subscribe<PolygonConnectedMessage>(Authenticate);
        }

        private void Authenticate(PolygonConnectedMessage message)
        {
            _logger.LogInformation($"Stocks: Authenticate happened");
            _client.Send($"{{\"action\":\"auth\",\"params\":\"{_appSettings.Value.Polygon.ApiKey}\"}}");
        }


        private void SubscribeToStreams(StreamReceiverAuthenticatedMessage message)
        {
            _logger.LogInformation($"Stocks: SubscribeToStreams happened");

            if (message.ServerType == DataServerType.Stocks)
            {
                _client.Send("{\"action\":\"subscribe\",\"params\":\"T.*\"}");
                _client.Send("{\"action\":\"subscribe\",\"params\":\"Q.*\"}");
                _client.Send("{\"action\":\"subscribe\",\"params\":\"A.*\"}");
            }
        }

        public async Task ConnectAsync()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_appSettings.Value.Polygon.ApiKey))
            {
                _logger.LogCritical("Polygon API Key has not found.");
                return;
            }

            var stocksUrl = _appSettings.Value.Polygon.StocksUrl;
            if (string.IsNullOrWhiteSpace(stocksUrl))
            {
                _logger.LogCritical("Polygon Url for stocks streaming has not found.");
                return;
            }

            var url = new Uri(stocksUrl);
            _client = new WebsocketClient(url);
            _disconnectionHappenedSubs = _client.DisconnectionHappened.Subscribe(OnDisconnection);
            _reconnectionHappenedSubs = _client.ReconnectionHappened.Subscribe(OnReconnection);

            _client.ReconnectTimeoutMs = (int) TimeSpan.FromSeconds(30).TotalMilliseconds;
            _client.ErrorReconnectTimeoutMs = (int) TimeSpan.FromSeconds(3).TotalMilliseconds;

            _messageReceivedSubs = _client.MessageReceived.Subscribe(OnMessage);
            await _client.Start();
        }

        private void OnReconnection(ReconnectionType type)
        {
            _logger.LogInformation($"Stocks: Reconnection happened, type: {type}");
        }

        private void OnDisconnection(DisconnectionType type)
        {
            _logger.LogInformation($"Stocks: Disconnection happened, type: {type}");
        }

        private void OnMessage(ResponseMessage message)
        {
            try
            {
                var streamingMessages = JsonConvert.DeserializeObject<IList<StreamingMessage>>(message.Text,
                    new PolygonMessageConverter());

                _streamMessageHandler.Handle(streamingMessages);
            }
            catch (Exception e)
            {
                _logger.LogError(e, message.Text);
            }
        }


        public void Dispose()
        {
            _messageReceivedSubs?.Dispose();
            _disconnectionHappenedSubs?.Dispose();
            _reconnectionHappenedSubs?.Dispose();
            _client.Dispose();
        }
    }
}