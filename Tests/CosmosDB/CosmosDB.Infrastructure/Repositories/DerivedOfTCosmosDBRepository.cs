using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using CosmosDB.Domain.Repositories.Documents;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal class DerivedOfTCosmosDBRepository : CosmosDBRepositoryBase<DerivedOfT, DerivedOfTDocument, IDerivedOfTDocument>, IDerivedOfTRepository
    {
        public DerivedOfTCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<DerivedOfTDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }
    }
}