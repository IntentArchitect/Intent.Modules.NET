using System;
using System.Collections.Concurrent;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMultiTenantConnectionFactory", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence
{
    public class MongoDbMultiTenantConnectionFactory
    {
        private readonly ConcurrentDictionary<string, CacheableMongoDb> _connectionCache;

        public MongoDbMultiTenantConnectionFactory()
        {
            _connectionCache = new ConcurrentDictionary<string, CacheableMongoDb>();
        }

        public IMongoDatabase GetConnection(string connectionString)
        {
            var db = new MongoClient(connectionString).GetDatabase(new MongoUrl(connectionString).DatabaseName);
            return _connectionCache.GetOrAdd(connectionString, id => new CacheableMongoDb(db)).GetDatabase();
        }

        private class CacheableMongoDb
        {
            public CacheableMongoDb(IMongoDatabase underlyingConnection)
            {
                UnderlyingConnection = underlyingConnection;
            }

            public IMongoDatabase UnderlyingConnection { get; }
            public IMongoClient Client => UnderlyingConnection.Client;

            public IMongoDatabase GetDatabase()
            {
                return UnderlyingConnection;
            }
        }
    }
}