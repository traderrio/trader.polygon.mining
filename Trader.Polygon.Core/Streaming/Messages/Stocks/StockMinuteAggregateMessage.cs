using Trader.Polygon.Core.Common.Enums;

namespace Trader.Polygon.Core.Streaming.Messages.Stocks
{
    public class StockMinuteAggregatedMessage: BaseStockAggregatedMessage
    {
        public override StreamingMessageType MessageType => StreamingMessageType.StockMinuteAggregated;
        public override DataServerType ServerType => DataServerType.Stocks;
    }
}