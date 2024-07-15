using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories
{
    internal class IdTestingCosmosDBRepository : CosmosDBRepositoryBase<IIdTesting, IdTesting, IdTestingDocument, IIdTestingDocument>, IIdTestingRepository
    {
        public IdTestingCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<IdTestingDocument> cosmosRepository,
            ICosmosContainerProvider<IdTestingDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "identifier", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<IIdTesting?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}