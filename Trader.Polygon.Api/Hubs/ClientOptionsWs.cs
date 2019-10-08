using System.Collections.Generic;
using MessagePack;
using Trader.Common.Product;
using Trader.Polygon.Core.Common.Enums;

namespace Trader.Polygon.Api.Hubs
{
    [MessagePackObject]
    public class ClientOptionsWs
    {
        public ClientOptionsWs()
        {
            Products = new List<StockProductModel>();
            MessageTypes = new List<StreamingMessageType>();
        }

        [Key("sendPricesBack")]
        public bool SendPricesBack { get; set; }

        [Key("products")]
        public IList<StockProductModel> Products { get; set; }

        [Key("allProducts")]
        public bool AllProducts { get; set; }

        [Key("messageTypes")]
        public IList<StreamingMessageType> MessageTypes { get; set; }
    }
}