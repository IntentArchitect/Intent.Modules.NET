using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IB_OptionalDependentRepository : IEFRepository<B_OptionalDependent, B_OptionalDependent>
    {
        [IntentManaged(Mode.Fully)]
        Task<B_OptionalDependent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<B_OptionalDependent?> FindByIdAsync(Guid id, Func<IQueryable<B_OptionalDependent>, IQueryable<B_OptionalDependent>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<B_OptionalDependent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}