using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPC.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPC.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_Poly_TopLevelRepository : IEFRepository<TPC_Poly_TopLevel, TPC_Poly_TopLevel>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_TopLevel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_TopLevel?> FindByIdAsync(Guid id, Func<IQueryable<TPC_Poly_TopLevel>, IQueryable<TPC_Poly_TopLevel>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_Poly_TopLevel>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}