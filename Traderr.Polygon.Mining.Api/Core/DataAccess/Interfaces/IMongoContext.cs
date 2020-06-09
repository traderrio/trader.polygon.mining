using MongoDB.Driver;
using Traderr.Polygon.Mining.Api.Core.Base.Interfaces;

namespace Traderr.Polygon.Mining.Api.Core.DataAccess.Interfaces
{
    public interface IMongoContext
    {
        /// <summary>
        /// Gets configured database
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// Gets database table manager <see cref="IMongoTable{T}"/>
        /// </summary>
        /// <typeparam name="T">Table key type</typeparam>
        /// <returns>Type based table object</returns>
        IMongoTable<T> Collection<T>() where T : IEntity, new();

        IMongoTable<T> GetDynamicNameCollection<T>(string name) where T : IEntity, new();
    }
}