using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.PrimaryKeyTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INewClassFloatRepository : IEFRepository<NewClassFloat, NewClassFloat>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(float id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NewClassFloat?> FindByIdAsync(float id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NewClassFloat?> FindByIdAsync(float id, Func<IQueryable<NewClassFloat>, IQueryable<NewClassFloat>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NewClassFloat>> FindByIdsAsync(float[] ids, CancellationToken cancellationToken = default);
    }
}