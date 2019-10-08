using Microsoft.AspNetCore.Mvc;
using Trader.Polygon.Api.MessageHandlers.Interfaces;

namespace Trader.Polygon.Api.Controllers
{
	[Route("api/[controller]")]
	public class WhoAmiController : Controller
	{
		private readonly IStreamMessageHandler _streamMessageHandler;

		public WhoAmiController(IStreamMessageHandler streamMessageHandler)
		{
			_streamMessageHandler = streamMessageHandler;
		}
		public IActionResult Get()
		{
			return Ok("Polygon Mining Operational");
		}
	}
}