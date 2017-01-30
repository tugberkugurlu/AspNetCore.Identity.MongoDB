using System;
using MongoDB.Driver;
using MongoDB.Testing.Mongo;

namespace AspNetCore.Identity.MongoDB.Tests.Common
{
    internal static class MongoDbServerTestUtils
    {
        public static DisposableDatabase CreateDatabase() =>
            new DisposableDatabase(MongoTestServer.Setup(27017));
        
        internal class DisposableDatabase : IDisposable
        {
            private readonly MongoTestServer _mongoTestServer;

            public DisposableDatabase(MongoTestServer mongoTestServer)
            {
                if (mongoTestServer == null) throw new ArgumentNullException(nameof(mongoTestServer));

                _mongoTestServer = mongoTestServer;
            }

            public IMongoDatabase Database => _mongoTestServer.Database;

            public void Dispose()
            {
                try
                {
                    _mongoTestServer.Dispose();
                }
                catch(ObjectDisposedException)
                {
                    /*
                        It's for some reason, disposing MongoTestServer can give us ObjectDisposedException. I have only
                        seen this happening on Travis CI. Let's swallow this for now.
                        
                          System.AggregateException : One or more errors occurred. (Cannot access a disposed object.
                          Object name: 'SingleServerCluster'.)
                          ---- System.ObjectDisposedException : Cannot access a disposed object.
                          Object name: 'SingleServerCluster'.
                          Stack Trace:
                               at System.Threading.Tasks.Task.ThrowIfExceptional(Boolean includeTaskCanceledExceptions)
                               at System.Threading.Tasks.Task.Wait(Int32 millisecondsTimeout, CancellationToken cancellationToken)
                               at MongoDB.Testing.Mongo.DefaultRandomMongoDatabase.Dispose()
                               at MongoDB.Testing.Mongo.MongoTestServer.Dispose()
                    */
                }
            }
        }
    }
}