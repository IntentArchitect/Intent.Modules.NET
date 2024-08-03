using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_FkDerivedClassRepository : IEFRepository<TPC_FkDerivedClass, TPC_FkDerivedClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_FkDerivedClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, CancellationToken cancellationToken = default);
    }
}