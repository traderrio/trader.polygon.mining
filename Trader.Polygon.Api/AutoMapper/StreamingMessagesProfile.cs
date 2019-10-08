using AutoMapper;
using Trader.Polygon.Api.Domain;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.AutoMapper
{
    public class StreamingMessagesProfile:Profile
    {
        public StreamingMessagesProfile()
        {
            CreateMap<StockLastTradeMessage, StockLastTrade>().ReverseMap();
            CreateMap<StockLastQuoteMessage, StockQuote>().ReverseMap();
            CreateMap<StockSecondAggregatedMessage, StockSecondAggregated>()
                .ForMember(d=>d.Start, opt => opt.MapFrom(s=>s.DateTime))
                .ReverseMap();
            
            CreateMap<StockMinuteAggregatedMessage, StockMinuteAggregated>()
                .ForMember(d=>d.Start, opt => opt.MapFrom(s=>s.DateTime))
                .ReverseMap();
        }
    }
}