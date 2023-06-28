using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal class WithoutPartitionKeyCosmosDBRepository : CosmosDBRepositoryBase<WithoutPartitionKey, WithoutPartitionKey, WithoutPartitionKeyDocument>, IWithoutPartitionKeyRepository
    {
        public WithoutPartitionKeyCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<WithoutPartitionKeyDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }
    }
}