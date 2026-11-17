using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.EF11.Domain
{
    public class Post
    {
        public Post()
        {
            Title = null!;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateOnly PublishedOn { get; set; }

        public Guid BlogId { get; set; }
    }
}