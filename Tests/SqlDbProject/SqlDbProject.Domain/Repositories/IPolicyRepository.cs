using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPolicyRepository : IEFRepository<Policy, Policy>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(long policyId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Policy?> FindByIdAsync(long policyId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Policy?> FindByIdAsync(long policyId, Func<IQueryable<Policy>, IQueryable<Policy>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Policy>> FindByIdsAsync(long[] policyIds, CancellationToken cancellationToken = default);
    }
}