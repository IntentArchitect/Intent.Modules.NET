using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Entities.Folder;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents.Folder;
using CosmosDB.EntityInterfaces.Domain.Repositories.Folder;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories.Folder
{
    internal class FolderContainerCosmosDBRepository : CosmosDBRepositoryBase<IFolderContainer, FolderContainer, FolderContainerDocument, IFolderContainerDocument>, IFolderContainerRepository
    {
        public FolderContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<FolderContainerDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<IFolderContainer?> FindByIdAsync(
            (string Id, string FolderPartitionKey) id,
            CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id.Id, partitionKey: id.FolderPartitionKey, cancellationToken: cancellationToken);
    }
}