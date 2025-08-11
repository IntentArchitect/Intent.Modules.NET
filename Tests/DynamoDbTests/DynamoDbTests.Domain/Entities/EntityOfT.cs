using DynamoDbTests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.Domain.Entities
{
    public class EntityOfT<T> : IHasDomainEvent
    {
        private string? _id;

        public EntityOfT()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public T GenericAttribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}