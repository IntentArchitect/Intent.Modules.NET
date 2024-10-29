using System;
using System.Collections.Concurrent;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoFramework;
using MongoFramework.Infrastructure.Diagnostics;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbConnectionFactory", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.Persistence
{
    public class MongoDbConnectionFactory : IDisposable
    {
        private readonly ConcurrentDictionary<string, CacheableMongoDbConnection> _connectionCache;

        public MongoDbConnectionFactory()
        {
            _connectionCache = new ConcurrentDictionary<string, CacheableMongoDbConnection>();
        }

        public IMongoDbConnection GetConnection(ITenantInfo tenant)
        {
            if (tenant is null || tenant.Identifier is null)
            {
                throw new ArgumentException("Tenant not supplied.", nameof(tenant));
            }
            return _connectionCache.GetOrAdd(tenant.Identifier, id => new CacheableMongoDbConnection(MongoDbConnection.FromConnectionString(tenant.ConnectionString)));
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