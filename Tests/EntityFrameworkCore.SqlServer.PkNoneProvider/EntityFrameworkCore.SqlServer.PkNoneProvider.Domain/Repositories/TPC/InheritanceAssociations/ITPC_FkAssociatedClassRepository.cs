using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_FkAssociatedClassRepository : IEFRepository<TPC_FkAssociatedClass, TPC_FkAssociatedClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_FkAssociatedClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPC_FkAssociatedClass?> FindByIdAsync(Guid id, Func<IQueryable<TPC_FkAssociatedClass>, IQueryable<TPC_FkAssociatedClass>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_FkAssociatedClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}