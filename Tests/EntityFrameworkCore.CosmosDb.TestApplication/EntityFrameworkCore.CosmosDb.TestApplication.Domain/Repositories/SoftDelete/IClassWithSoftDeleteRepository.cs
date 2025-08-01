using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.SoftDelete;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.SoftDelete
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClassWithSoftDeleteRepository : IEFRepository<ClassWithSoftDelete, ClassWithSoftDelete>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ClassWithSoftDelete?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ClassWithSoftDelete?> FindByIdAsync(Guid id, Func<IQueryable<ClassWithSoftDelete>, IQueryable<ClassWithSoftDelete>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ClassWithSoftDelete>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}