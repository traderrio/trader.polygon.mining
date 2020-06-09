using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Traderr.Polygon.Mining.Api.Core.Base.Interfaces;

namespace Traderr.Polygon.Mining.Api.Core.Base
{
    public abstract class Entity : IEntity
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonIgnore]
        public abstract string CollectionName { get; }
    }
}