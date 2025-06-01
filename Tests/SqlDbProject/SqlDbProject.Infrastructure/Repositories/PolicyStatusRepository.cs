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
    public class PolicyStatusRepository : RepositoryBase<PolicyStatus, PolicyStatus, ApplicationDbContext>, IPolicyStatusRepository
    {
        public PolicyStatusRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid policyStatusId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.PolicyStatusId == policyStatusId, cancellationToken);
        }

        public async Task<PolicyStatus?> FindByIdAsync(Guid policyStatusId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PolicyStatusId == policyStatusId, cancellationToken);
        }

        public async Task<PolicyStatus?> FindByIdAsync(
            Guid policyStatusId,
            Func<IQueryable<PolicyStatus>, IQueryable<PolicyStatus>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PolicyStatusId == policyStatusId, queryOptions, cancellationToken);
        }

        public async Task<List<PolicyStatus>> FindByIdsAsync(
            Guid[] policyStatusIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = policyStatusIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.PolicyStatusId), cancellationToken);
        }
    }
}