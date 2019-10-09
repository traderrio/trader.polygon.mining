using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Trader.Polygon.Api.DataSavers.Interfaces;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Api.Infrastructure;

namespace Trader.Polygon.Api.Controllers
{
    [Route("api/stock-last-trade")]
    public class StockLastTradeController : Controller
    {
        private readonly IPolygonDataSaver _dataSaver;

        public StockLastTradeController(IPolygonDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        [HttpPost]
        public async Task<BinaryIntegrationResponse> Saves([FromBody]List<StockLastTrade> stockLastTrades)
        {
            try
            {
                await _dataSaver.SaveStockLastTradesAsync(stockLastTrades);
                var serializedMessage = MessagePackSerializer.Serialize("OK",
                    MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                return BinaryIntegrationResponse.Ok(serializedMessage);
            }
            catch (Exception e)
            {
                return BinaryIntegrationResponse.BadRequest(e.Message);
            }
        }
    }
}