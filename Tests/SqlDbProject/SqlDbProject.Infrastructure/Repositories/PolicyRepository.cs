using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PolicyRepository : RepositoryBase<Policy, Policy, ApplicationDbContext>, IPolicyRepository
    {
        public PolicyRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            long policyId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.PolicyId == policyId, cancellationToken);
        }

        public async Task<Policy?> FindByIdAsync(long policyId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PolicyId == policyId, cancellationToken);
        }

        public async Task<Policy?> FindByIdAsync(
            long policyId,
            Func<IQueryable<Policy>, IQueryable<Policy>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PolicyId == policyId, queryOptions, cancellationToken);
        }

        public async Task<List<Policy>> FindByIdsAsync(long[] policyIds, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = policyIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.PolicyId), cancellationToken);
        }
    }
}