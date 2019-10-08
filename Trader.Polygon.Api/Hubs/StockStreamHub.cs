using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Trader.Common.Product;
using Trader.Polygon.Api.Services.Interfaces;
using Trader.Polygon.Core.Common.Enums;
using Trader.Polygon.Core.Streaming.Messages;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.Hubs
{
    public class StockPricesHub : Hub
    {
        private readonly ILastStockPriceMemoryHolder _stockPriceMemoryHolder;
        private ConcurrentDictionary<string, ClientOptionsWs> RegisteredClients { get; set; }

        public StockPricesHub(ILastStockPriceMemoryHolder stockPriceMemoryHolder)
        {
            _stockPriceMemoryHolder = stockPriceMemoryHolder;
            RegisteredClients = new ConcurrentDictionary<string, ClientOptionsWs>();
        }

        public async Task HandleStockProductsMessage(ClientOptionsWs optionsWs)
        {
            RegisterProduct(optionsWs);

            if (optionsWs.SendPricesBack)
            {
                SendLastPricesToClient(optionsWs.Products);
            }

            await Task.CompletedTask;
        }


        private void RegisterProduct(ClientOptionsWs options)
        {
            var connectionId = Context.ConnectionId;
            RegisteredClients[connectionId] = options;
        }


        public void SendQuotes(List<StockLastQuoteMessage> prices)
        {
            var clients = RegisteredClients
                .Where(c => c.Value.MessageTypes.Contains(StreamingMessageType.StockLastQuote)).ToList();

            foreach (var client in clients)
            {
                Send(client, prices);
            }
        }

        public void SendLastTrades(List<StockLastTradeMessage> prices)
        {
            var clients = RegisteredClients
                .Where(c => c.Value.MessageTypes.Contains(StreamingMessageType.StockLastTrade)).ToList();

            foreach (var client in clients)
            {
                Send(client, prices);
            }
        }

        public void SendAggregated(List<StockSecondAggregatedMessage> prices)
        {
            var clients = RegisteredClients
                .Where(c => c.Value.MessageTypes.Contains(StreamingMessageType.StockSecondAggregated)).ToList();

            foreach (var client in clients)
            {
                Send(client, prices);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            RegisteredClients.TryRemove(connectionId, out _);

            await base.OnDisconnectedAsync(exception);
        }

        private void Send<T>(KeyValuePair<string, ClientOptionsWs> client, IEnumerable<T> prices)
            where T : StreamingMessage

        {
            if (client.Value.AllProducts)
            {
                Clients.Client(client.Key).SendAsync("AssetsPricesUpdated", prices);
            }
            else
            {
                var stocksPrices = prices.Where(p => client.Value.Products.Any(s => s.Ticker == p.Ticker))
                    .ToArray();

                if (stocksPrices.Length > 0)
                {
                    Clients.Client(client.Key).SendAsync("AssetsPricesUpdated", stocksPrices);
                }
            }
        }

        private void SendLastPricesToClient(IList<StockProductModel> newProducts)
        {
            var connectionId = Context.ConnectionId;
            var tickers = newProducts.Select(s => s.Ticker).ToList();
            var allStocksMessages = _stockPriceMemoryHolder.FindMessages(tickers).Result;

            if (allStocksMessages.Count == 0)
            {
                return;
            }

            var aggregatedMessages = new List<StockSecondAggregatedMessage>();

            var clientAggregatedMessages = allStocksMessages.Where(a => a.Aggregated != null)
                .Select(a => a.Aggregated).ToList();
            aggregatedMessages.AddRange(clientAggregatedMessages);

            if (aggregatedMessages.Count > 0)
            {
                Clients.Client(connectionId).SendAsync("AssetsPricesUpdated", aggregatedMessages.ToArray());
            }

            var lastTradeMessages = new List<StockLastTradeMessage>();

            var clientLastTradeMessages = allStocksMessages.Where(a => a.LastTrade != null)
                .Select(a => a.LastTrade).ToList();
            lastTradeMessages.AddRange(clientLastTradeMessages);

            if (lastTradeMessages.Count > 0)
            {
                Clients.Client(connectionId).SendAsync("AssetsPricesUpdated", lastTradeMessages.ToArray());
            }

            var quoteMessages = new List<StockLastQuoteMessage>();
            var clientQuoteMessages = allStocksMessages.Where(a => a.Quote != null)
                .Select(a => a.Quote).ToList();
            quoteMessages.AddRange(clientQuoteMessages);

            if (quoteMessages.Count > 0)
            {
                Clients.Client(connectionId).SendAsync("AssetsPricesUpdated", quoteMessages.ToArray());
            }
        }
    }
}