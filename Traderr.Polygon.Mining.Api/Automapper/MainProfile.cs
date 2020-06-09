using AutoMapper;
using Traderr.Polygon.Mining.Api.Domain;
using Traderr.Polygon.Mining.Api.Polygon;
using Traderr.Polygon.Mining.Api.Polygon.Models;

namespace Traderr.Polygon.Mining.Api.Automapper
{
    public class MainProfile:Profile
    {
        public MainProfile()
        {
            CreateMap<PolygonHistoricalNbboQuoteModel, StockHistoricalQuote>();
        }
    }
}