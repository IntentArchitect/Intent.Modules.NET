using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Contracts
{
    public record LineDataContract
    {
        public LineDataContract(string description, int quantity)
        {
            Description = description;
            Quantity = quantity;
        }

        public string Description { get; init; }
        public int Quantity { get; init; }
    }
}