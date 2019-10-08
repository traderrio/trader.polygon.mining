using MessagePack;
using Newtonsoft.Json;
using Trader.Common.MessagePackFormatters;
using Trader.Polygon.Core.Common.Enums;

namespace Trader.Polygon.Core.Streaming.Messages.Stocks
{
    [MessagePackObject]
    public class StockLastTradeMessage:StreamingMessage
    {
        public override StreamingMessageType MessageType => StreamingMessageType.StockLastTrade;
        public override DataServerType ServerType => DataServerType.Stocks;

        [JsonProperty("x")]
        [IgnoreMember]
        public int Exchange { get; set; }
        
        [JsonProperty("p")]    
        [Key("p")]
        [MessagePackFormatter(typeof(DecimalFormatter))]
        public decimal Price { get; set; }
        
        [JsonProperty("s")]
        [IgnoreMember]
        public long Size { get; set; }
        
       

    }
}