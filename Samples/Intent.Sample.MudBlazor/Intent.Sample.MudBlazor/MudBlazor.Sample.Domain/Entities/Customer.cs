using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MudBlazor.Sample.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Customer()
        {
            Name = null!;
            Address = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? AccountNo { get; set; }

        public Address Address { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}