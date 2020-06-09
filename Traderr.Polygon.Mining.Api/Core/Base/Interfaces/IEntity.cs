using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Traderr.Polygon.Mining.Api.Core.Base.Interfaces
{
    public interface IEntity
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }

        [BsonIgnore]
        string CollectionName { get; }
    }
}