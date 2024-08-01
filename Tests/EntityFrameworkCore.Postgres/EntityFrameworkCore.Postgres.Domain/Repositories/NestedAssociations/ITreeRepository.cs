using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITreeRepository : IEFRepository<Tree, Tree>
    {
        [IntentManaged(Mode.Fully)]
        Task<Tree?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Tree>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}