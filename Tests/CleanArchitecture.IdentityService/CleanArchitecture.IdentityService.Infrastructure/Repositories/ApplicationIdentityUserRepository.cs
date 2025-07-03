using AutoMapper;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Domain.Repositories;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ApplicationIdentityUserRepository : RepositoryBase<ApplicationIdentityUser, ApplicationIdentityUser, ApplicationDbContext>, IApplicationIdentityUserRepository
    {

        public ApplicationIdentityUserRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<ApplicationIdentityUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ApplicationIdentityUser?> FindByIdAsync(
            string id,
            Func<IQueryable<ApplicationIdentityUser>, IQueryable<ApplicationIdentityUser>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ApplicationIdentityUser>> FindByIdsAsync(
            string[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}