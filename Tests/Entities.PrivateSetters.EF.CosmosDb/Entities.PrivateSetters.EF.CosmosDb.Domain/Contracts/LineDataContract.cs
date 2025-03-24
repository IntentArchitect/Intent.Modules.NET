using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Domain.Contracts
{
    public record LineDataContract
    {
        public LineDataContract(string description, int quantity)
        {
            Description = description;
            Quantity = quantity;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected LineDataContract()
        {
            Description = null!;
        }

        public string Description { get; init; }
        public int Quantity { get; init; }
    }
}