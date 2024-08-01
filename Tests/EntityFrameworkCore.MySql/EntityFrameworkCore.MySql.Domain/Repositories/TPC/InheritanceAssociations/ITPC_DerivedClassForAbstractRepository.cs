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
    public interface ITPC_DerivedClassForAbstractRepository : IEFRepository<TPC_DerivedClassForAbstract, TPC_DerivedClassForAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_DerivedClassForAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_DerivedClassForAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}