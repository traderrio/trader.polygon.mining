using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Traderr.Polygon.Mining.Api.Core.Base.Interfaces;
using Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces;

namespace Traderr.Polygon.Mining.Api.Core.DataAccess
{
    /// <summary>
    /// Represents mongo type based table manager
    /// </summary>
    /// <typeparam name="T">Table key type</typeparam>
    public class MongoTable<T> : IDisposable, IMongoTable<T> where T:IEntity
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
        /// <param name="content">Table content</param>
        public MongoTable(IMongoCollection<T> content)
        {
            Content = content;
        }

        /// <summary>
        /// Table content.
        /// MongoDB.Driver IMongoCollection standard functionality.
        /// </summary>
        public IMongoCollection<T> Content { get; set; }

        /// <summary>
        /// Table filter.
        /// MongoDB.Driver FilterDefinitionBuilder standard functionality.
        /// </summary>
        public FilterDefinitionBuilder<T> Filter = new FilterDefinitionBuilder<T>();

        /// <summary>
        /// Getting fixed filter
        /// </summary>
        /// <param name="filter">Raw filter</param>
        /// <returns>Fixed filter</returns>
        private FilterDefinition<T> FixFilter(FilterDefinition<T> filter = null)
        {
            // If filter is not defined
            if (filter == null)
            {
                // We are fixing it by default value
                filter = Builders<T>.Filter.Empty;
            }
            // returning fixed filter
            return filter;
        }

        /// <summary>
        /// Get IQueryable interface
        /// </summary>
        /// <returns></returns>
        public IMongoQueryable<T> AsQueryable()
        {
            return Content.AsQueryable();
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="filter">data filter</param>
        /// <returns>data count</returns>
        public async Task<long> CountAsync(FilterDefinition<T> filter = null)
        {
            // Finxing filter
            filter = FixFilter(filter);
            // Returning data count
            return await Content.CountDocumentsAsync(filter);
        }

        /// <summary>
        /// Loading data
        /// </summary>
        /// <param name="filter">data filter</param>
        /// <param name="kit">data filter kit</param>
        /// <returns>filtered data</returns>
        public async Task<List<T>> LoadAsync(FilterDefinition<T> filter = null, MongoFilterKit kit = null)
        {
            filter = FixFilter(filter);
            
            // Defining options according kit
            var options = kit == null ? new FindOptions<T> { } : kit.ToFindOptions<T>();
            
            // In case if limit is not requested
            if (options.Limit == 0)
            {
                // Limit will become null
                // This means that all possible records will be materialized
                options.Limit = null;
            }
            // In case if sorting is not defined
            if (options.Sort == null)
            {
                // We create default sorting parameters
                options.Sort = new BsonDocument("_id", 1);
            }
            
            var data = await Content.FindAsync(filter, options);
            
            return data.ToList();
        }

        /// <summary>
        /// Get item
        /// </summary>
        /// <param name="filter">Record filter</param>
        /// <returns>Filtered item</returns>
        public async Task<T> GetAsync(FilterDefinition<T> filter)
        {
            // Loading data with only one record kit
            var data = await LoadAsync(filter, new MongoFilterKit { Skip = 0, Limit = 1 });
            // Returning that single record
            return data.FirstOrDefault();
        }

        /// <summary>
        /// Insert item
        /// </summary>
        /// <param name="item">data item</param>
        /// <returns>Nothing</returns>
        public async Task InsertAsync(T item)
        {
            if (item != null)
            {
                await Content.InsertOneAsync(item);
            }
        }


        /// <summary>
        /// Insert data
        /// </summary>
        /// <param name="items">data items</param>
        /// <returns>Nothing</returns>
        public async Task InsertAsync(IEnumerable<T> items)
        {
            var documents = items.ToList();
            
            if (documents.Any())
            {
                await Content.InsertManyAsync(documents);
            }
        }
        
        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Nothing</returns>
        public async Task<ReplaceOneResult> UpdateAsync(T item)
        {
            var filter = Filter.Eq(f => f.Id, item.Id);
            return await Content.ReplaceOneAsync(filter, item);
        }
        
        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="filter">Data filter</param>
        /// <returns>Nothing</returns>
        public async Task DeleteAsync(FilterDefinition<T> filter = null)
        {
            filter = FixFilter(filter);
            await Content.DeleteManyAsync(filter);
        }
        
        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Nothing</returns>
        public async Task DeleteAsync(string id)
        {
            var filter = Filter.Eq(t => t.Id, id);
            await Content.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Drop collection
        /// </summary>
        /// <returns></returns>
        public async Task DropAsync()
        {
           await Content.Database.DropCollectionAsync(Content.CollectionNamespace.CollectionName);
        }
    }

}
