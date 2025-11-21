using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.BasicAudit;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.BasicAudit
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAudit_DerivedClassRepository : IEFRepository<Audit_DerivedClass, Audit_DerivedClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<Audit_DerivedClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Audit_DerivedClass?> FindByIdAsync(Guid id, Func<IQueryable<Audit_DerivedClass>, IQueryable<Audit_DerivedClass>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Audit_DerivedClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}