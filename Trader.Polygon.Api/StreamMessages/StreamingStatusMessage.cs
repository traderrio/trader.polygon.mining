using Newtonsoft.Json;
using Trader.Polygon.Core.Common.Enums;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.StreamMessages
{
    public class StreamingStatusMessage : StreamingMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        public override StreamingMessageType MessageType => StreamingMessageType.Status;
        public override DataServerType ServerType => DataServerType.Stocks;

    }
}