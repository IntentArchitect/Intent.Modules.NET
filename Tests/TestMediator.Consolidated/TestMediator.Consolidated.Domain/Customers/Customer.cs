using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.Consolidated.Domain.Customers
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
            Email = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}