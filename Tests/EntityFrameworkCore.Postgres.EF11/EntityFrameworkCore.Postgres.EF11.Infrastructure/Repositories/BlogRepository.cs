using EntityFrameworkCore.Postgres.EF11.Domain;
using EntityFrameworkCore.Postgres.EF11.Domain.Repositories;
using EntityFrameworkCore.Postgres.EF11.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlogRepository : RepositoryBase<Blog, Blog, ApplicationDbContext>, IBlogRepository
    {
        public BlogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Blog?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Blog?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Blog>, IQueryable<Blog>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Blog>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }

        [IntentManaged(Mode.Ignore)]
        public async Task<List<Blog>> FindWithPostsPublishedSinceAsync(
            DateOnly since,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(
                x => x.Posts!.Any(p => p.PublishedOn >= since),
                q => q.Include(x => x.Posts!.Where(p => p.PublishedOn >= since)),
                cancellationToken);
        }
    }
}
