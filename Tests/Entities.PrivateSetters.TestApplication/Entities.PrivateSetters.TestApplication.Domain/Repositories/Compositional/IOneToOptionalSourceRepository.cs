using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOneToOptionalSourceRepository : IEFRepository<OneToOptionalSource, OneToOptionalSource>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<OneToOptionalSource?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<OneToOptionalSource?> FindByIdAsync(Guid id, Func<IQueryable<OneToOptionalSource>, IQueryable<OneToOptionalSource>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OneToOptionalSource>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}