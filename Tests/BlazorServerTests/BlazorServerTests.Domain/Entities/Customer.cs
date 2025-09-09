using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace BlazorServerTests.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}