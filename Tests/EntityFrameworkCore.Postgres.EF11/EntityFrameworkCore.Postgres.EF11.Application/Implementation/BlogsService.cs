using EntityFrameworkCore.Postgres.EF11.Application.Interfaces;
using EntityFrameworkCore.Postgres.EF11.Domain;
using EntityFrameworkCore.Postgres.EF11.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class BlogsService : IBlogsService
    {
        private readonly IBlogRepository _blogRepository;

        [IntentManaged(Mode.Ignore)]
        public BlogsService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        /// <summary>
        /// Creates a blog with its first post.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateBlog(CreateBlogDto dto, CancellationToken cancellationToken = default)
        {
            var blog = new Blog
            {
                Name = dto.Name,
                Posts = [new Post { Title = dto.FirstPostTitle, PublishedOn = dto.FirstPostPublishedOn }]
            };

            _blogRepository.Add(blog);
            await _blogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return blog.Id;
        }

        /// <summary>
        /// Returns blogs that have posts published on or after 'since', counting only those posts (filtered Include).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<BlogSummaryDto>> GetBlogsWithPostsSince(
            DateOnly since,
            CancellationToken cancellationToken = default)
        {
            var blogs = await _blogRepository.FindWithPostsPublishedSinceAsync(since, cancellationToken);
            return blogs
                .Select(b => new BlogSummaryDto { Id = b.Id, Name = b.Name, PostCount = b.Posts?.Count ?? 0 })
                .ToList();
        }
    }
}
