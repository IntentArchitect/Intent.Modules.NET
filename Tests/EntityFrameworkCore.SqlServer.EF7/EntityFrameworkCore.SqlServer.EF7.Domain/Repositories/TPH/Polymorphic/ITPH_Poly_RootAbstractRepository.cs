using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_Poly_RootAbstractRepository : IEFRepository<TPH_Poly_RootAbstract, TPH_Poly_RootAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_RootAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_RootAbstract?> FindByIdAsync(Guid id, Func<IQueryable<TPH_Poly_RootAbstract>, IQueryable<TPH_Poly_RootAbstract>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_Poly_RootAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}