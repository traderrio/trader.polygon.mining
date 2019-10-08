using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Trader.Polygon.Api.Infrastructure;
using Trader.Polygon.Api.Services;

namespace Trader.Polygon.Api.Controllers
{
    [Route("api/trading-activity")]
    public class TradingActivityController : Controller
    {
        private readonly ITradingActivityService _tradingActivityService;

        public TradingActivityController(ITradingActivityService tradingActivityService)
        {
            _tradingActivityService = tradingActivityService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("value");
        }

        [HttpPost]
        public async Task<BinaryIntegrationResponse> Calculate([FromBody]TradingActivityQuery query)
        {
            var activities = await _tradingActivityService.CalculateTimeFramedAsync(query);
            var serializedMessage = MessagePackSerializer.Serialize(activities,
                MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            return BinaryIntegrationResponse.Ok(serializedMessage);
        }
    }
}