using System;
using MessagePack;
using Newtonsoft.Json;
using Trader.Common.MessagePackFormatters;

namespace Trader.Polygon.Core.Streaming.Messages.Stocks
{
    [MessagePackObject]
    public abstract class BaseStockAggregatedMessage : StreamingMessage
    {
        [JsonProperty("op")]
        [Key("o")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal Open { get; set; }

        [JsonProperty("c")]
        [Key("c")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal Close { get; set; }

        [JsonProperty("h")]
        [Key("h")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal High { get; set; }

        [JsonProperty("l")]
        [Key("l")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal Low { get; set; }

        [JsonProperty("a")]
        [IgnoreMember]
        public decimal Average { get; set; }

        [JsonProperty("s")]
        [JsonConverter(typeof(EpochInMillisecondsJsonConverter))]
        [Key("t")]
        public override DateTime DateTime{ get; set; }

        [JsonProperty("e")]
        [JsonConverter(typeof(EpochInMillisecondsJsonConverter))]
        [IgnoreMember]
        public DateTime End { get; set; }

        [JsonProperty("v")]
        [Key("v")]
        public long Volume { get; set; }

        [JsonProperty("av")]
        [IgnoreMember]
        public long AccumulatedVolume { get; set; }

        [JsonProperty("vw")]
        [IgnoreMember]
        public long Vwap { get; set; }
    }
}