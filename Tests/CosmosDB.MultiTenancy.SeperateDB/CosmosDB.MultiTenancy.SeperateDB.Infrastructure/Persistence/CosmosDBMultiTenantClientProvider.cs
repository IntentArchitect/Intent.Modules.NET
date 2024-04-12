using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBMultiTenantClientProvider", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence
{
    public class CosmosDBMultiTenantClientProvider : ICosmosClientProvider, IDisposable
    {
        private readonly AsyncLocal<ScopedData?> _scopedData = new AsyncLocal<ScopedData?>();
        private readonly Dictionary<string, ConnectionInfo> _clients = new Dictionary<string, ConnectionInfo>();
        private readonly object _lock = new object();
        private readonly string _defaultDatabaseName;
        private readonly string _defaultContainerName;
        private readonly IConfiguration _configuration;

        public CosmosDBMultiTenantClientProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            var repositoryOptions = new RepositoryOptions();
            configuration.GetSection("RepositoryOptions").Bind(repositoryOptions);
            _defaultContainerName = repositoryOptions.ContainerId;
            _defaultDatabaseName = repositoryOptions.DatabaseId;
        }

        public ITenantInfo? Tenant => _scopedData?.Value?.Tenant;
        public CosmosClient CosmosClient => GetClient();

        public string GetDatabase()
        {
            return GetTenantConnectionInfo().Database;
        }

        public string GetDefaultContainer()
        {
            return GetTenantConnectionInfo().DefaultContainer;
        }

        private CosmosClient GetClient()
        {
            return GetTenantConnectionInfo().Client;
        }

        private ConnectionInfo GetTenantConnectionInfo()
        {
            if (_scopedData.Value == null || _scopedData.Value.Tenant == null || _scopedData.Value.Tenant.Id == null)
            {
                throw new ArgumentNullException("Tenant info not found, unable to determine which database to access");
            }
            var tenantInfo = _scopedData.Value.Tenant;
            if (!_clients.TryGetValue(tenantInfo.Id, out var connectionInfo))
            {
                lock (_lock)
                {
                    if (!_clients.TryGetValue(tenantInfo.Id, out connectionInfo))
                    {
                        string[] settings = tenantInfo.ConnectionString.Split(";");
                        var clientOptions = _scopedData.Value.ClientOptions;
                        var client = new CosmosClient(tenantInfo.ConnectionString, clientOptions);
                        GetValuesFromConnectionString(tenantInfo.ConnectionString, out var database, out var defaultContainer);
                        if (database == null)
                        {
                            database = _defaultDatabaseName;
                        }
                        if (defaultContainer == null)
                        {
                            defaultContainer = _defaultContainerName;
                        }
                        connectionInfo = new ConnectionInfo(client, database, defaultContainer);
                        _clients.Add(tenantInfo.Id, connectionInfo);
                    }
                }
            }
            return connectionInfo;
        }

        private void GetValuesFromConnectionString(
            string connectionString,
            out string? database,
            out string? defaultContainer)
        {
            database = null;
            defaultContainer = null;
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
            if (builder.TryGetValue("Database", out object dbValue))
            {
                database = dbValue.ToString();
            }
            if (builder.TryGetValue("Container", out object containerValue))
            {
                defaultContainer = containerValue.ToString();
            }
        }

        public void Dispose()
        {
            foreach (var connection in _clients.Values)
            {
                connection.Client.Dispose();
            }
        }

        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) => consume.Invoke(GetClient());

        public IDisposable SetLocalState(ITenantInfo? tenant, ICosmosClientOptionsProvider cosmosClientOptionsProvider)
        {
            _scopedData.Value ??= new ScopedData(tenant, cosmosClientOptionsProvider, () => _scopedData.Value = null);
            return _scopedData.Value;
        }

        private class ConnectionInfo
        {
            public ConnectionInfo(CosmosClient client, string database, string defaultContainer)
            {
                Client = client;
                Database = database;
                DefaultContainer = defaultContainer;
            }

            public CosmosClient Client { get; }
            public string Database { get; }
            public string DefaultContainer { get; }
        }
        private class ScopedData : IDisposable
        {
            private readonly Action _disposeAction;

            public ScopedData(ITenantInfo? tenant,
                ICosmosClientOptionsProvider cosmosClientOptionsProvider,
                Action disposeAction)
            {
                Tenant = tenant;
                _disposeAction = disposeAction;
                ClientOptions = cosmosClientOptionsProvider.ClientOptions;
            }

            public ITenantInfo? Tenant { get; }
            public CosmosClientOptions ClientOptions { get; }

            public void Dispose() => _disposeAction();
        }
    }
}