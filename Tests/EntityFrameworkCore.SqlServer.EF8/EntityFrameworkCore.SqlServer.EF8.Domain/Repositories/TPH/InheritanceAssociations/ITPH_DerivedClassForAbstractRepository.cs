using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_DerivedClassForAbstractRepository : IEFRepository<TPH_DerivedClassForAbstract, TPH_DerivedClassForAbstract>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_DerivedClassForAbstract?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_DerivedClassForAbstract?> FindByIdAsync(Guid id, Func<IQueryable<TPH_DerivedClassForAbstract>, IQueryable<TPH_DerivedClassForAbstract>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_DerivedClassForAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}