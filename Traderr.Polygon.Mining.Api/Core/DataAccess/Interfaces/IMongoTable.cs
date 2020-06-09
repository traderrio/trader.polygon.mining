using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Traderr.Polygon.Mining.Api.Core.Base.Interfaces;

namespace Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces
{
    public interface IMongoTable<T> where T : IEntity
    {
        /// <summary>
        /// Ending class lifecycle
        /// </summary>
        void Dispose();

        /// <summary>
        /// Table content.
        /// MongoDB.Driver IMongoCollection standard functionality.
        /// </summary>
        IMongoCollection<T> Content { get; set; }

        IMongoQueryable<T> AsQueryable();

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="filter">data filter</param>
        /// <returns>data count</returns>
        Task<long> CountAsync(FilterDefinition<T> filter = null);

        /// <summary>
        /// Loading data
        /// </summary>
        /// <param name="filter">data filter</param>
        /// <param name="kit">data filter kit</param>
        /// <returns>filtered data</returns>
        Task<List<T>> LoadAsync(FilterDefinition<T> filter = null, MongoFilterKit kit = null);

        /// <summary>
        /// Get item
        /// </summary>
        /// <param name="filter">Record filter</param>
        /// <returns>Filtered item</returns>
        Task<T> GetAsync(FilterDefinition<T> filter);

        /// <summary>
        /// Insert item
        /// </summary>
        /// <param name="item">data item</param>
        /// <returns>Nothing</returns>
        Task InsertAsync(T item);

        /// <summary>
        /// Insert data
        /// </summary>
        /// <param name="items">data items</param>
        /// <returns>Nothing</returns>
        Task InsertAsync(IEnumerable<T> items);

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Nothing</returns>
        Task<ReplaceOneResult> UpdateAsync(T item);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="filter">Data filter</param>
        /// <returns>Nothing</returns>
        Task DeleteAsync(FilterDefinition<T> filter = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Nothing</returns>
        Task DeleteAsync(string id);

        Task DropAsync();
    }
}