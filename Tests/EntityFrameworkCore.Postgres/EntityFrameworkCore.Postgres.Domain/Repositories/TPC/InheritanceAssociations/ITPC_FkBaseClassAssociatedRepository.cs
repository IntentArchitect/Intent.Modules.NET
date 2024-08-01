using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_FkBaseClassAssociatedRepository : IEFRepository<TPC_FkBaseClassAssociated, TPC_FkBaseClassAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_FkBaseClassAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_FkBaseClassAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}