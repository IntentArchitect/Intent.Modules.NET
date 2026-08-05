using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.SQLLite.Tests.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            FirstName = null!;
            LastName = null!;
            Email = null!;
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}