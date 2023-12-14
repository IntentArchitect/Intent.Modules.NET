using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IOrderItem
    {
        string Id { get; set; }

        int Quantity { get; set; }

        string Description { get; set; }

        decimal Amount { get; set; }
    }
}