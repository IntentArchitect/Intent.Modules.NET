using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents.ManyToMany;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Repositories.ManyToMany
{
    internal class ProductItemCosmosDBRepository : CosmosDBRepositoryBase<ProductItem, ProductItemDocument, IProductItemDocument>, IProductItemRepository
    {
        public ProductItemCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<ProductItemDocument> cosmosRepository,
            ICosmosContainerProvider<ProductItemDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            IMapper mapper) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, mapper)
        {
        }

        public async Task<ProductItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<ProductItem>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}