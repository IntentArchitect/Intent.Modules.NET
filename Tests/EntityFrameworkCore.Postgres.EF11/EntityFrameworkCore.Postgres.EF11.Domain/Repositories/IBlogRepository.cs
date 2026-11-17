using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBlogRepository : IEFRepository<Blog, Blog>
    {
        [IntentManaged(Mode.Fully)]
        Task<Blog?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Blog?> FindByIdAsync(Guid id, Func<IQueryable<Blog>, IQueryable<Blog>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Blog>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
        /// <summary>
        /// Blogs with at least one post published on or after <paramref name="since"/>, with only those posts loaded.
        /// </summary>
        Task<List<Blog>> FindWithPostsPublishedSinceAsync(DateOnly since, CancellationToken cancellationToken = default);
    }
}
