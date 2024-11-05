using System;
using System.Collections.Concurrent;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoFramework;
using MongoFramework.Infrastructure.Diagnostics;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMultiTenantConnectionFactory", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence
{
    public class MongoDbMultiTenantConnectionFactory : IDisposable
    {
        private readonly ConcurrentDictionary<string, CacheableMongoDbConnection> _connectionCache;

        public MongoDbMultiTenantConnectionFactory()
        {
            _connectionCache = new ConcurrentDictionary<string, CacheableMongoDbConnection>();
        }

        public IMongoDbConnection GetConnection(string connectionString)
        {
            return _connectionCache.GetOrAdd(connectionString, id => new CacheableMongoDbConnection(MongoDbConnection.FromConnectionString(connectionString)));
        }

        public void Dispose()
        {
            foreach (var connection in _connectionCache.Values)
            {
                connection.UnderlyingConnection.Dispose();
            }
        }

        private class CacheableMongoDbConnection : IMongoDbConnection
        {
            public CacheableMongoDbConnection(IMongoDbConnection underlyingConnection)
            {
                UnderlyingConnection = underlyingConnection;
            }

            public IMongoDbConnection UnderlyingConnection { get; }
            public IMongoClient Client => UnderlyingConnection.Client;
            public IDiagnosticListener DiagnosticListener
            {
                get => UnderlyingConnection.DiagnosticListener;
                set => UnderlyingConnection.DiagnosticListener = value;
            }

            public IMongoDatabase GetDatabase()
            {
                return UnderlyingConnection.GetDatabase();
            }

            public void Dispose()
            {
                //DI is forcing `IMongoDbConnection` to make these scoped, but we want to cache these per tenant
                //This stops the container from disposing this on every request
            }
        }
    }
}