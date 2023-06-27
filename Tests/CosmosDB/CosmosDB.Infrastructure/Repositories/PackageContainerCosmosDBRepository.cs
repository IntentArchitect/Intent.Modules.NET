using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal class PackageContainerCosmosDBRepository : CosmosDBRepositoryBase<PackageContainer, PackageContainer, PackageContainerDocument>, IPackageContainerRepository
    {
        public PackageContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<PackageContainerDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }
    }
}