using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface ILineItem
    {
        string Id { get; set; }

        string Description { get; set; }

        int Quantity { get; set; }

        string ProductId { get; set; }

        ICollection<string> Tags { get; set; }
    }
}