using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.BugFixes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.BugFixes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITaskRepository : IEFRepository<Entities.BugFixes.Task, Entities.BugFixes.Task>
    {
        [IntentManaged(Mode.Fully)]
        System.Threading.Tasks.Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Entities.BugFixes.Task?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Entities.BugFixes.Task?> FindByIdAsync(Guid id, Func<IQueryable<Entities.BugFixes.Task>, IQueryable<Entities.BugFixes.Task>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Entities.BugFixes.Task>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}