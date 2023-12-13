using System.Threading;
using System.Threading.Tasks;
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
    internal class BaseTypeCosmosDBRepository : CosmosDBRepositoryBase<BaseType, BaseTypeDocument, IBaseTypeDocument>, IBaseTypeRepository
    {
        public BaseTypeCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<BaseTypeDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<BaseType?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}