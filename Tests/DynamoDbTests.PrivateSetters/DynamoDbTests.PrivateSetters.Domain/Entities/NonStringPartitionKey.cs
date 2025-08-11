using DynamoDbTests.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities
{
    public class NonStringPartitionKey : IHasDomainEvent
    {
        private string? _id;
        private int? _partInt;
        public NonStringPartitionKey()
        {
            Id = null!;
            Name = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public int PartInt
        {
            get => _partInt ?? throw new NullReferenceException("_partInt has not been set");
            private set => _partInt = value;
        }

        public string Name { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}