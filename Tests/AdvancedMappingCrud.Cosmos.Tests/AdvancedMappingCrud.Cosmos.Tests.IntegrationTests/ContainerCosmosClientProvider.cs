using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Providers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ContainerCosmosClientProvider", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests
{
    public class ContainerCosmosClientProvider : ICosmosClientProvider, IDisposable
    {
        private readonly CosmosClient _cosmosClient;

        public ContainerCosmosClientProvider(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public CosmosClient CosmosClient => _cosmosClient;

        public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume) => consume.Invoke(_cosmosClient);

        public void Dispose()
        {
            if (_cosmosClient != null)
            {
                _cosmosClient.Dispose();
            }
        }
    }
}