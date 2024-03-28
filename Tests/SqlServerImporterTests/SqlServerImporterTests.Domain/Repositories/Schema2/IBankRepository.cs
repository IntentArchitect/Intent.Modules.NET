using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Entities.Schema2;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Repositories.Schema2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBankRepository : IEFRepository<Bank, Bank>
    {
        [IntentManaged(Mode.Fully)]
        Task<Bank?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Bank>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}