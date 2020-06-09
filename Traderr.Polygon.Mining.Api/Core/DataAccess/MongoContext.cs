using System;
using Dawn;
using MongoDB.Driver;
using Traderr.Polygon.Mining.Api.Core.Base.Interfaces;
using Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces;
using Traderr.Polygon.Mining.Api.Domain;

namespace Traderr.Polygon.Mining.Api.Core.DataAccess
{
    public class MongoContext : IDisposable, IMongoContext
    {
        /// <summary>
        /// Ending class lifecycle
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration <see cref="MongoConfiguration"/></param>
        public MongoContext(MongoConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration object
        /// </summary>
        private MongoConfiguration Configuration { get; set; }

        /// <summary>
        /// Database object
        /// </summary>
        private IMongoDatabase DatabaseObject { get; set; }

        /// <summary>
        /// Gets configured database
        /// </summary>
        public IMongoDatabase Database
        {
            get
            {
                if (DatabaseObject == null)
                {
                    var client = new MongoClient(Configuration.Settings);
                    DatabaseObject = client.GetDatabase(Configuration.Database);
                }

                return DatabaseObject;
            }
        }

        /// <summary>
        /// Gets database table manager <see cref="MongoTable{T}"/>
        /// </summary>
        /// <typeparam name="T">Table key type</typeparam>
        /// <returns>Type based table object</returns>
        public IMongoTable<T> Collection<T>() where T : IEntity, new()
        {
            var collectionName = new T().CollectionName;
            Guard.Argument(collectionName).NotEmpty();
            return new MongoTable<T>(Database.GetCollection<T>(collectionName));
        }
        
        public IMongoTable<T> GetDynamicNameCollection<T>(string name) where T : IEntity, new()
        {
            Guard.Argument(name).NotEmpty();
            string collectionName = new T().CollectionName;
            Guard.Argument(collectionName).NotEmpty();

            return new MongoTable<T>(Database.GetCollection<T>(name + "-" + collectionName));
        }
    }
}