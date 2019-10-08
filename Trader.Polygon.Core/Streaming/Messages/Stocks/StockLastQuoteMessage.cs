using MessagePack;
using Newtonsoft.Json;
using Trader.Common.MessagePackFormatters;
using Trader.Polygon.Core.Common.Enums;

namespace Trader.Polygon.Core.Streaming.Messages.Stocks
{
    [MessagePackObject]
    public class StockLastQuoteMessage: StreamingMessage
    {
        public override StreamingMessageType MessageType => StreamingMessageType.StockLastQuote;
        public override DataServerType ServerType => DataServerType.Stocks;
        
        [JsonProperty("ap")]
        [Key("ap")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal Ask { get; set; }
        
        [JsonProperty("bp")]
        [Key("bp")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal Bid { get; set; }
        
        [JsonProperty("as")]
        [Key("as")]
        public long AskSize { get; set; }
        
        [JsonProperty("bs")]
        [Key("bs")]
        public long BidSize { get; set; }
        
        [JsonProperty("ax")]
        [IgnoreMember]
        public int AskExchange { get; set; }
        
        [JsonProperty("bx")]
        [IgnoreMember]
        public int BidExchange { get; set; }
    }
}