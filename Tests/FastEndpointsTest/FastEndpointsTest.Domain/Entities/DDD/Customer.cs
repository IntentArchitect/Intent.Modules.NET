using FastEndpointsTest.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.DDD
{
    public class Customer : Person
    {
        public Customer()
        {
            Email = null!;
            Address = null!;
        }
        public string Email { get; set; }

        public Address Address { get; set; }
    }
}