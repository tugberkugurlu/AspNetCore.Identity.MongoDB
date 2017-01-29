using MongoDB.Driver;

namespace AspNetCore.Identity.MongoDB.Tests.Common
{
    internal static class MongoDatabaseExtensions
    {
        public static IMongoCollection<MongoIdentityUser> GetDefaultCollection(this IMongoDatabase database) =>
            database.GetCollection<MongoIdentityUser>(Constants.DefaultCollectionName);
    }
}