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
    internal class BaseTypeCosmosDBRepository : CosmosDBRepositoryBase<BaseType, BaseType, BaseTypeDocument>, IBaseTypeRepository
    {
        public BaseTypeCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<BaseTypeDocument> cosmosRepository) : base(unitOfWork, cosmosRepository, "id")
        {
        }
    }
}