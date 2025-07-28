using DynamoDbTests.EnumAsStrings.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EnumAsStrings.Domain.Entities
{
    public class WithoutPartitionKey : IHasDomainEvent
    {
        private string? _id;
        public WithoutPartitionKey()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}