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
    public interface IH_MultipleDependentRepository : IEFRepository<H_MultipleDependent, H_MultipleDependent>
    {
        [IntentManaged(Mode.Fully)]
        Task<H_MultipleDependent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<H_MultipleDependent?> FindByIdAsync(Guid id, Func<IQueryable<H_MultipleDependent>, IQueryable<H_MultipleDependent>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<H_MultipleDependent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}