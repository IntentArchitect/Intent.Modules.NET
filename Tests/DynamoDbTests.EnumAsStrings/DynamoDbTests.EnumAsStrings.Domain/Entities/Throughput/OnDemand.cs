using DynamoDbTests.EnumAsStrings.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EnumAsStrings.Domain.Entities.Throughput
{
    public class OnDemand : IHasDomainEvent
    {
        private string? _id;
        public OnDemand()
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