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
    internal class IdTestingCosmosDBRepository : CosmosDBRepositoryBase<IdTesting, IdTesting, IdTestingDocument>, IIdTestingRepository
    {
        public IdTestingCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<IdTestingDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "identifier")
        {
        }
    }
}