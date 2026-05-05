using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DtoSettings.Class.Protected.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace DtoSettings.Class.Protected.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDefaultValueEntityRepository : IEFRepository<DefaultValueEntity, DefaultValueEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultValueEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultValueEntity?> FindByIdAsync(Guid id, Func<IQueryable<DefaultValueEntity>, IQueryable<DefaultValueEntity>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DefaultValueEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}