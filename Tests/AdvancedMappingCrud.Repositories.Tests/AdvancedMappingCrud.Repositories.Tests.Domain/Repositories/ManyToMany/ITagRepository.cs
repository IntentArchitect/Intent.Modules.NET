using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ManyToMany
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITagRepository : IEFRepository<Entities.ManyToMany.Tag, Entities.ManyToMany.Tag>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Entities.ManyToMany.Tag?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Entities.ManyToMany.Tag?> FindByIdAsync(Guid id, Func<IQueryable<Entities.ManyToMany.Tag>, IQueryable<Entities.ManyToMany.Tag>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Entities.ManyToMany.Tag>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}