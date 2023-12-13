using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Domain.Entities.Folder;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Folder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderContainerRepository : ICosmosDBRepository<FolderContainer, IFolderContainerDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<FolderContainer?> FindByIdAsync((string Id, string FolderPartitionKey) id, CancellationToken cancellationToken = default);
    }
}