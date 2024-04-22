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
    public interface IParentRepository : IEFRepository<Parent, Parent>
    {
        [IntentManaged(Mode.Fully)]
        Task<Parent?> FindByIdAsync((Guid Id, Guid Id2) id, CancellationToken cancellationToken = default);
    }
}