using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence;
using AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Repositories
{
    internal class ProductCosmosDBRepository : CosmosDBRepositoryBase<Product, ProductDocument, IProductDocument>, IProductRepository
    {
        public ProductCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<ProductDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }
    }
}