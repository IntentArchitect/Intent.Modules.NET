using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Entities.Folder;
using CosmosDB.Domain.Repositories.Documents.Folder;
using CosmosDB.Domain.Repositories.Folder;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories.Folder
{
    internal class FolderContainerCosmosDBRepository : CosmosDBRepositoryBase<FolderContainer, FolderContainerDocument, IFolderContainerDocument>, IFolderContainerRepository
    {
        public FolderContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<FolderContainerDocument> cosmosRepository,
            ICosmosContainerProvider<FolderContainerDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor, currentUserService)
        {
        }

        public async Task<FolderContainer?> FindByIdAsync(
            (string Id, string FolderPartitionKey) id,
            CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.Id, partitionKey: id.FolderPartitionKey, cancellationToken: cancellationToken);
    }
}