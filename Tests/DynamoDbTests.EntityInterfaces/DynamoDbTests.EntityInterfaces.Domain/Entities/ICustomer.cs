using DynamoDbTests.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public interface ICustomer : IHasDomainEvent
    {
        string Id { get; set; }

        string Name { get; set; }

        IList<string>? Tags { get; set; }

        Address DeliveryAddress { get; set; }

        Address? BillingAddress { get; set; }
    }
}