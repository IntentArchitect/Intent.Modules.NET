using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SQLLite.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
            Suranme = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Suranme { get; set; }
    }
}