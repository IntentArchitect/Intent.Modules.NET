using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IChildRepository : IEFRepository<Child, Child>
    {
        [IntentManaged(Mode.Fully)]
        Task<Child?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Child>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}