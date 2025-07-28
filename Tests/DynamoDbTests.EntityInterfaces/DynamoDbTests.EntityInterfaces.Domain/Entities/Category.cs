using DynamoDbTests.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class Category : ICategory, IHasDomainEvent
    {
        private string? _id;

        public Category()
        {
            Id = null!;
            Name = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}