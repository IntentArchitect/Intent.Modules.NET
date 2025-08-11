using DynamoDbTests.EntityInterfaces.Domain.Common;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class Product : IProduct, IHasDomainEvent
    {
        private string? _id;

        public Product()
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

        public IList<string> CategoriesIds { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}