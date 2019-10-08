using System.Collections.Generic;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.MessageHandlers.ServerSpecific.Interfaces
{
    public interface IServerMessageHandler
    {
        void Handle(IList<StreamingMessage> streamingMessages);
    }
}