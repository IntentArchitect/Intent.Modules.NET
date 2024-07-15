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
    internal class ClassContainerCosmosDBRepository : CosmosDBRepositoryBase<IClassContainer, ClassContainer, ClassContainerDocument, IClassContainerDocument>, IClassContainerRepository
    {
        public ClassContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<ClassContainerDocument> cosmosRepository,
            ICosmosContainerProvider<ClassContainerDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<IClassContainer?> FindByIdAsync(
            (string Id, string ClassPartitionKey) id,
            CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.Id, partitionKey: id.ClassPartitionKey, cancellationToken: cancellationToken);
    }
}