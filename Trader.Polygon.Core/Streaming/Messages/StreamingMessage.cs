using System;
using MessagePack;
using Newtonsoft.Json;
using Trader.Polygon.Core.Common.Enums;

namespace Trader.Polygon.Core.Streaming.Messages
{
    [MessagePackObject]
    public abstract class StreamingMessage
    {
        [Key("mt")]
        public abstract StreamingMessageType MessageType { get; }
        
        [IgnoreMember]
        public abstract DataServerType ServerType { get;  }
        
        [JsonProperty("sym")]
        [Key("sym")]
        public string Ticker { get; set; }
        
        [JsonConverter(typeof(EpochInMillisecondsJsonConverter))]
        [JsonProperty("t")]
        [Key("t")]
        public virtual DateTime DateTime { get; set; }

    }
}