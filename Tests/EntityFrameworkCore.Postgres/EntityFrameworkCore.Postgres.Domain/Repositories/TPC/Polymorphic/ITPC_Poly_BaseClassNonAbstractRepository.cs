using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPC.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_Poly_BaseClassNonAbstractRepository : IEFRepository<TPC_Poly_BaseClassNonAbstract, TPC_Poly_BaseClassNonAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_BaseClassNonAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_BaseClassNonAbstract?> FindByIdAsync(Guid id, Func<IQueryable<TPC_Poly_BaseClassNonAbstract>, IQueryable<TPC_Poly_BaseClassNonAbstract>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_Poly_BaseClassNonAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}