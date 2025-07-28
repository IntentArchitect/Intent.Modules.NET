using DynamoDbTests.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities
{
    public class University : IHasDomainEvent
    {
        private Guid? _id;

        public University()
        {
            Name = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            private set => _id = value;
        }

        public string Name { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}