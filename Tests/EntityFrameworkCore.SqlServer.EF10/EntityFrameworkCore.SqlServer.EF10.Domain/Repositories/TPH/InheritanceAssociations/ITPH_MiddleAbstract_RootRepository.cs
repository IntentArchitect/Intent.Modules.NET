using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_MiddleAbstract_RootRepository : IEFRepository<TPH_MiddleAbstract_Root, TPH_MiddleAbstract_Root>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_MiddleAbstract_Root?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_MiddleAbstract_Root?> FindByIdAsync(Guid id, Func<IQueryable<TPH_MiddleAbstract_Root>, IQueryable<TPH_MiddleAbstract_Root>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_MiddleAbstract_Root>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}