using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBMultitenantContainerProvider", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence
{
    public class CosmosDBMultitenantContainerProvider<TItem> : ICosmosContainerProvider<TItem>
        where TItem : IItem
    {
        private readonly ConcurrentDictionary<string, Container> _containers = new ConcurrentDictionary<string, Container>();
        private readonly CosmosDBMultiTenantClientProvider _clientProvider;
        private readonly ICosmosContainerService _containerService;

        public CosmosDBMultitenantContainerProvider(ICosmosContainerService containerService,
            ICosmosClientProvider clientProvider)
        {
            _containerService = containerService;
            _clientProvider = (CosmosDBMultiTenantClientProvider)clientProvider;
        }

        public async Task<Container> GetContainerAsync()
        {
            if (_clientProvider.Tenant == null || _clientProvider.Tenant.Id == null)
            {
                throw new Exception("Tenant Info missing for Connection lookup");
            }

            var tenantInfo = _clientProvider.Tenant;
            if (!_containers.TryGetValue(tenantInfo.Id, out var container))
            {
                container = await _containerService.GetContainerAsync<TItem>();
                _containers.AddOrUpdate(tenantInfo.Id, container, (k, c) => c);
            }
            return container;
        }
    }
}