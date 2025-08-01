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
    public interface ITPC_Poly_RootAbstract_AggrRepository : IEFRepository<TPC_Poly_RootAbstract_Aggr, TPC_Poly_RootAbstract_Aggr>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_RootAbstract_Aggr?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_RootAbstract_Aggr?> FindByIdAsync(Guid id, Func<IQueryable<TPC_Poly_RootAbstract_Aggr>, IQueryable<TPC_Poly_RootAbstract_Aggr>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_Poly_RootAbstract_Aggr>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}