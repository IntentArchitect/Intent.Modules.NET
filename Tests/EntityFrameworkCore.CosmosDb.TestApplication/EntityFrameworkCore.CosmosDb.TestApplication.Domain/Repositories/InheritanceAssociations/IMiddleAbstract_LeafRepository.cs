using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMiddleAbstract_LeafRepository : IEFRepository<MiddleAbstract_Leaf, MiddleAbstract_Leaf>
    {
        [IntentManaged(Mode.Fully)]
        Task<MiddleAbstract_Leaf?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MiddleAbstract_Leaf>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}