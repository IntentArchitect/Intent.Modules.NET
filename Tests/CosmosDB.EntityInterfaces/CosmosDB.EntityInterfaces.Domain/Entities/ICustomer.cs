using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface ICustomer : IHasDomainEvent
    {
        string Id { get; set; }

        string Name { get; set; }

        ICollection<string>? Tags { get; set; }

        Address DeliveryAddress { get; set; }

        Address? BillingAddress { get; set; }
    }
}