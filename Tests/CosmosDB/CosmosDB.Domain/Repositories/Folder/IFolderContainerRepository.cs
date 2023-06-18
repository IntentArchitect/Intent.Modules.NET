using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Folder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderContainerRepository : ICosmosDBRepository<FolderContainer, FolderContainer>
    {
    }
}