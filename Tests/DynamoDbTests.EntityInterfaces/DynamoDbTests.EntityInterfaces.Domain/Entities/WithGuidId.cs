using DynamoDbTests.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class WithGuidId : IWithGuidId, IHasDomainEvent
    {
        private Guid? _id;

        public WithGuidId()
        {
            Field = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Field { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}