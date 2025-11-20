using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IF_OptionalDependentRepository : IEFRepository<F_OptionalDependent, F_OptionalDependent>
    {
        [IntentManaged(Mode.Fully)]
        Task<F_OptionalDependent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<F_OptionalDependent?> FindByIdAsync(Guid id, Func<IQueryable<F_OptionalDependent>, IQueryable<F_OptionalDependent>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<F_OptionalDependent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}