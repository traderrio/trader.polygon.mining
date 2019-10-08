using Trader.Polygon.Core.Common.Enums;

namespace Trader.Polygon.Core.Streaming.Messages.Stocks
{
    public class StockSecondAggregatedMessage: BaseStockAggregatedMessage
    {
        public override StreamingMessageType MessageType => StreamingMessageType.StockSecondAggregated;
        public override DataServerType ServerType => DataServerType.Stocks;
    }
}