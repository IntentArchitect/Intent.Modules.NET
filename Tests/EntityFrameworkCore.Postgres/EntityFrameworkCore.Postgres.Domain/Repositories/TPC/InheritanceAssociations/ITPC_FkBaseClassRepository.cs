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
    public interface ITPC_FkBaseClassRepository : IEFRepository<TPC_FkBaseClass, TPC_FkBaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_FkBaseClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, CancellationToken cancellationToken = default);
    }
}