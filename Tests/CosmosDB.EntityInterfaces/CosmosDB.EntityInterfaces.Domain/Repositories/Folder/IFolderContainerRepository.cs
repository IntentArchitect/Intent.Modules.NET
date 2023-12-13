using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Domain.Entities.Folder;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Repositories.Folder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderContainerRepository : ICosmosDBRepository<IFolderContainer, IFolderContainerDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<IFolderContainer?> FindByIdAsync((string Id, string FolderPartitionKey) id, CancellationToken cancellationToken = default);
    }
}