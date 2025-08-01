using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_Poly_RootAbstract_AggrRepository : IEFRepository<TPH_Poly_RootAbstract_Aggr, TPH_Poly_RootAbstract_Aggr>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_RootAbstract_Aggr?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_RootAbstract_Aggr?> FindByIdAsync(Guid id, Func<IQueryable<TPH_Poly_RootAbstract_Aggr>, IQueryable<TPH_Poly_RootAbstract_Aggr>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_Poly_RootAbstract_Aggr>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}