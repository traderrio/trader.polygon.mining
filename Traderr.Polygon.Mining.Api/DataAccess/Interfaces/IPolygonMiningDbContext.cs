﻿using MongoDB.Driver;
 using Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces;
 using Traderr.Polygon.Mining.Api.Domain;

 namespace Traderr.Polygon.Mining.Api.DataAccess.Interfaces
{
    public interface IPolygonMiningDbContext: IMongoContext
    {
        void CreateHistoricalQuoteIndex(IMongoTable<StockHistoricalQuote> mongoTable);
    }
}