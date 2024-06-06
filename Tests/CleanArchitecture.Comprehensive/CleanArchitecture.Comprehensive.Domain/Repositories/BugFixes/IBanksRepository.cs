using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.BugFixes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.BugFixes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBanksRepository : IEFRepository<Banks, Banks>
    {
        [IntentManaged(Mode.Fully)]
        Task<Banks?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Banks>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}