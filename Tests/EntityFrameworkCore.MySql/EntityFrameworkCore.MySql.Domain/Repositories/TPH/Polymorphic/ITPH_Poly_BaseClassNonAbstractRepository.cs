using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_Poly_BaseClassNonAbstractRepository : IEFRepository<TPH_Poly_BaseClassNonAbstract, TPH_Poly_BaseClassNonAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_BaseClassNonAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_BaseClassNonAbstract?> FindByIdAsync(Guid id, Func<IQueryable<TPH_Poly_BaseClassNonAbstract>, IQueryable<TPH_Poly_BaseClassNonAbstract>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_Poly_BaseClassNonAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}