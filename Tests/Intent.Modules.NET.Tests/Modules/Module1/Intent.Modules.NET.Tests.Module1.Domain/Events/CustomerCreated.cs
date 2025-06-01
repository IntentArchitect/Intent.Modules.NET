using Intent.Modules.NET.Tests.Module1.Domain.Common;
using Intent.Modules.NET.Tests.Module1.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Domain.Events
{
    public class CustomerCreated : DomainEvent
    {
        public CustomerCreated(Customer customer)
        {
            Customer = customer;
        }

        public Customer Customer { get; }
    }
}