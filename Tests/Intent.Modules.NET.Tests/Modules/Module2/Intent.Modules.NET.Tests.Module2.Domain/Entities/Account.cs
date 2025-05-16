using Intent.Modules.NET.Tests.Module2.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Domain.Entities
{
    public class Account : IHasDomainEvent
    {
        public Account()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}