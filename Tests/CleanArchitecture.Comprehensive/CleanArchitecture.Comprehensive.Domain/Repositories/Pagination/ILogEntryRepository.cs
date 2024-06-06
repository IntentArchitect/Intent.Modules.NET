using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.Pagination
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ILogEntryRepository : IEFRepository<LogEntry, LogEntry>
    {
        [IntentManaged(Mode.Fully)]
        Task<LogEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<LogEntry>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}