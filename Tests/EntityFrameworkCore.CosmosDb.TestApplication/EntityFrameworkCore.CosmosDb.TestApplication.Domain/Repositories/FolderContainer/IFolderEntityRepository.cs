using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.FolderContainer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.FolderContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderEntityRepository : IEFRepository<FolderEntity, FolderEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<FolderEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FolderEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}