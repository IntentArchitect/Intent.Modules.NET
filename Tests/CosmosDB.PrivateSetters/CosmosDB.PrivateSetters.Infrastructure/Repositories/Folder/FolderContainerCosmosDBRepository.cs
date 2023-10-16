using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain.Entities.Folder;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents.Folder;
using CosmosDB.PrivateSetters.Domain.Repositories.Folder;
using CosmosDB.PrivateSetters.Infrastructure.Persistence;
using CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Repositories.Folder
{
    internal class FolderContainerCosmosDBRepository : CosmosDBRepositoryBase<FolderContainer, FolderContainerDocument, IFolderContainerDocument>, IFolderContainerRepository
    {
        public FolderContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<FolderContainerDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }
    }
}