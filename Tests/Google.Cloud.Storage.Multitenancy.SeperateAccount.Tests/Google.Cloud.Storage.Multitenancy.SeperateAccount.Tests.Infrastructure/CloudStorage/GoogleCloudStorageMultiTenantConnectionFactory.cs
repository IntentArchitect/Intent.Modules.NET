using System;
using System.Collections.Concurrent;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.GoogleCloudStorageMultiTenantConnectionFactory", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.CloudStorage
{
    public class GoogleCloudStorageMultiTenantConnectionFactory : IDisposable
    {
        private readonly ConcurrentDictionary<string, StorageClient> _connectionCache;

        public GoogleCloudStorageMultiTenantConnectionFactory()
        {
            _connectionCache = new ConcurrentDictionary<string, StorageClient>();
        }

        public StorageClient GetStorageClient(string tenantId, string connectionJson)
        {
            return _connectionCache.GetOrAdd(tenantId, id =>
                    {
                        var googleCredential = GoogleCredential.FromJson(connectionJson);
                        return StorageClient.Create(googleCredential);
                    });
        }

        public void Dispose()
        {
            foreach (var connection in _connectionCache.Values)
            {
                connection.Dispose();
            }
        }
    }
}