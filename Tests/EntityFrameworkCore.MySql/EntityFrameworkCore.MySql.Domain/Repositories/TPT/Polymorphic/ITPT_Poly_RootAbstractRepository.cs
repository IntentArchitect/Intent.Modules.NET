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
    public interface ITPT_Poly_RootAbstractRepository : IEFRepository<TPT_Poly_RootAbstract, TPT_Poly_RootAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_RootAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_RootAbstract?> FindByIdAsync(Guid id, Func<IQueryable<TPT_Poly_RootAbstract>, IQueryable<TPT_Poly_RootAbstract>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_Poly_RootAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}