using MongoDB.Bson;
using MongoDB.Driver;

namespace Traderr.Polygon.Mining.Api.Core.DataAccess
{
    /// <summary>
    /// Represents portion and order part of mongo filter <see cref="FindOptions"/>
    /// </summary>
    public class MongoFilterKit
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public MongoFilterKit()
        {
            // By default order filed will be key and order direction will be descending
            OrderBy = "_id";
            OrderDirection = "desc";
        }

        /// <summary>
        /// Converting to MongoDB.Driver native FindOptions
        /// </summary>
        /// <typeparam name="T">Mongo table entity <see cref="MongoTableEntity{T}"/></typeparam>
        /// <returns>MongoDB.Driver native FindOptions</returns>
        public FindOptions<T> ToFindOptions<T>()
        {
            // Generating and returning FindOptions
            return new FindOptions<T>
            {
                Skip = Skip,
                Limit = Limit,
                Sort = new BsonDocument(OrderByToCapital, OrderDirectionToNumber)
            };
        }

        /// <summary>
        /// Skip ordered data
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Data portion size
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Order field
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Capitalized order field for actual use in mongo queries
        /// </summary>
        internal string OrderByToCapital
        {
            get
            {
                return OrderBy[0].ToString().ToUpper() + OrderBy.Remove(0, 1);
            }
        }

        /// <summary>
        /// Order direction
        /// </summary>
        public string OrderDirection { get; set; }

        /// <summary>
        /// Number representation of order direction for actual use in mongo queries
        /// </summary>
        internal int OrderDirectionToNumber
        {
            get
            {
                return OrderDirection.ToLower() == "desc" ? -1 : 1;
            }
        }

    }
}