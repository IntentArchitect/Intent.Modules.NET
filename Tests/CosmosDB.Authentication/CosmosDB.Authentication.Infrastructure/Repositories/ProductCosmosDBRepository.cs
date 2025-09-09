using CosmosDB.Authentication.Domain.Entities;
using CosmosDB.Authentication.Domain.Repositories;
using CosmosDB.Authentication.Domain.Repositories.Documents;
using CosmosDB.Authentication.Infrastructure.Persistence;
using CosmosDB.Authentication.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Authentication.Infrastructure.Repositories
{
    internal class ProductCosmosDBRepository : CosmosDBRepositoryBase<Product, ProductDocument, IProductDocument>, IProductRepository
    {
        public ProductCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<ProductDocument> cosmosRepository,
            ICosmosContainerProvider<ProductDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<Product?> FindByIdAsync(
            (string Id, string Name) id,
            CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.Id, partitionKey: id.Name, cancellationToken: cancellationToken);
    }
}