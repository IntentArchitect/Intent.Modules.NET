using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Application.Interfaces
{
    public interface IBlogsService
    {
        /// <summary>
        /// Creates a blog with its first post.
        /// </summary>
        Task<Guid> CreateBlog(CreateBlogDto dto, CancellationToken cancellationToken = default);
        /// <summary>
        /// Returns blogs that have posts published on or after 'since', counting only those posts (filtered Include).
        /// </summary>
        Task<List<BlogSummaryDto>> GetBlogsWithPostsSince(DateOnly since, CancellationToken cancellationToken = default);
    }
}