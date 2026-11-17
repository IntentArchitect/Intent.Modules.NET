using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class Book
    {
        public Book()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Author> Authors { get; set; } = [];
    }
}