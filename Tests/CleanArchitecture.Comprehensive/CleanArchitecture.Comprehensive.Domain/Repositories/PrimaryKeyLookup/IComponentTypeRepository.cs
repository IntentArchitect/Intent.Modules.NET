using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.PrimaryKeyLookup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IComponentTypeRepository : IEFRepository<ComponentType, ComponentType>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int componentTypeId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ComponentType?> FindByIdAsync(int componentTypeId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ComponentType?> FindByIdAsync(int componentTypeId, Func<IQueryable<ComponentType>, IQueryable<ComponentType>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ComponentType>> FindByIdsAsync(int[] componentTypeIds, CancellationToken cancellationToken = default);
    }
}