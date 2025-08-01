using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_Poly_BaseClassNonAbstractRepository : IEFRepository<TPT_Poly_BaseClassNonAbstract, TPT_Poly_BaseClassNonAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_BaseClassNonAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_BaseClassNonAbstract?> FindByIdAsync(Guid id, Func<IQueryable<TPT_Poly_BaseClassNonAbstract>, IQueryable<TPT_Poly_BaseClassNonAbstract>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_Poly_BaseClassNonAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}