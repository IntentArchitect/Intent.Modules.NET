using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.BasicAudit;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.BasicAudit
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAudit_SoloClassRepository : IEFRepository<Audit_SoloClass, Audit_SoloClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<Audit_SoloClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Audit_SoloClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}