using System;
using MongoDB.Driver;

namespace AspNetCore.Identity.MongoDB.Tests.Common
{
    internal static class MongoDbServerTestUtils
    {
        public static DisposableDatabase CreateDatabase() => new DisposableDatabase();

        public class DisposableDatabase : IDisposable
        {
            private bool _disposed;
            private readonly IMongoDatabase _database;
            private readonly MongoClient _mongoClient;

            public DisposableDatabase()
            {
                var databaseName = Guid.NewGuid().ToString("N");

                _mongoClient = new MongoClient("mongodb://localhost:27017");
                _database = _mongoClient.GetDatabase(databaseName);
            }

            public IMongoDatabase Database => _database;

            public void Dispose()
            {
                if (_disposed == false)
                {
                    _mongoClient.DropDatabase(_database.DatabaseNamespace.DatabaseName);
                    _disposed = true;
                }
            }
        }
    }
}