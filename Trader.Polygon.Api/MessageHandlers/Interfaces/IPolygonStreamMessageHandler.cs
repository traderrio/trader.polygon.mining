using System.Collections.Generic;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.MessageHandlers.Interfaces
{
    public interface IStreamMessageHandler
    {
        void Handle(IList<StreamingMessage> streamingMessages);
    }
}