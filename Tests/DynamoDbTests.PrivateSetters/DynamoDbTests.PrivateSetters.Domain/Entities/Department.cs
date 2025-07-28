using DynamoDbTests.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities
{
    public class Department : IHasDomainEvent
    {
        private Guid? _id;

        public Department()
        {
            Name = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            private set => _id = value;
        }

        public Guid? UniversityId { get; private set; }

        public string Name { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}