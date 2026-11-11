using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Indexes
{
    public class SingleChildIndex
    {
        public SingleChildIndex()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}