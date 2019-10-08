using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Trader.Polygon.Api.MessageHandlers.Interfaces;
using Trader.Polygon.Api.MessageHandlers.ServerSpecific;
using Trader.Polygon.Api.MessageHandlers.ServerSpecific.Interfaces;
using Trader.Polygon.Core.Common.Enums;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.MessageHandlers
{
    public class StreamMessageHandler : IStreamMessageHandler
    {
        private readonly IDictionary<DataServerType, IServerMessageHandler>
            _lookupServerHandler;
  

        public StreamMessageHandler(IServiceProvider serviceProvider)
        {
            var stockServerMessageHandler = (IServerMessageHandler) serviceProvider
                .GetRequiredService(typeof(StockServerMessageHandler));

            if (stockServerMessageHandler == null)
            {
                throw new NullReferenceException(nameof(stockServerMessageHandler));
            }
            
            _lookupServerHandler = new Dictionary<DataServerType, IServerMessageHandler>
            {
                {
                    DataServerType.Stocks,
                    stockServerMessageHandler
                }
            };
        }

        public void Handle(IList<StreamingMessage> streamingMessages)
        {
            var grouped = streamingMessages.GroupBy(m => m.ServerType);

            foreach (var message in grouped)
            {
                if (_lookupServerHandler.TryGetValue(message.Key, out var handler))
                {
                    handler.Handle(message.ToList());
                }
            }
        }
    }
}