using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Entities.Folder;
using CosmosDB.Domain.Repositories.Folder;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents.Folder;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using CosmosRepository = Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories.Folder
{
    internal class FolderContainerCosmosDBRepository : CosmosDBRepositoryBase<FolderContainer, FolderContainer, FolderContainerDocument>, IFolderContainerRepository
    {
        public FolderContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<FolderContainerDocument> cosmosRepository,
            IMapper mapper) : base(unitOfWork, cosmosRepository, mapper, "id")
        {
        }

        protected override void EnsureHasId(FolderContainer entity)
        {
            entity.Id ??= Guid.NewGuid().ToString();
        }
    }
}