﻿using MongoDB.Driver;

 namespace Traderr.Polygon.Mining.Api.Core.DataAccess
{
    /// <summary>
    /// Represents mongo configuration <see cref="MongoContext"/>
    /// </summary>
    public class MongoConfiguration
    {
        public MongoConfiguration(string url)
        {
            var mongoUrl = MongoUrl.Create(url);
            Settings = MongoClientSettings.FromUrl(mongoUrl);
            Database = mongoUrl.DatabaseName;
        }
        
        /// <summary>
        /// Connection settings
        /// </summary>
        public MongoClientSettings Settings { get; private set; }


        /// <summary>
        /// Actual database name
        /// </summary>
        public string Database { get; }
    }
}