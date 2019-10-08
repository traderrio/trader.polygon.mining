using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Trader.Polygon.Api.MessageHandlers.Interfaces;
using Trader.Polygon.Core.Streaming.Messages;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

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
			return Ok("Polygon is Operational");
		}
		
		
		[Route("pub")]
		public IActionResult Pub()
		{
			_streamMessageHandler.Handle(new List<StreamingMessage>
			{
				new StockLastTradeMessage
				{
					Ticker = "GOOG",
					DateTime = DateTime.Now,
					Price = 1086.35M,
					Exchange = 1,
					Size = 10
				},
				new StockLastQuoteMessage
				{
					Ticker = "GOOG",
					DateTime = DateTime.Now,
					Ask = 103,
					Bid = 99,
					AskSize = 1,
					BidSize = 1
				},
				new StockSecondAggregatedMessage
				{
					Ticker = "GOOG",
					DateTime = DateTime.Now,
					Open = 1119.99M,
					Close = 1102.33M,
					High = 1120.12M,
					Low = 1104.74M,
				}
			});
			
			return Ok("Polygon is Operational");
		}
	}
}