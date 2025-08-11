using DynamoDbTests.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class PackageContainer : IPackageContainer, IHasDomainEvent
    {
        private string? _id;
        private string? _packagePartitionKey;
        public PackageContainer()
        {
            Id = null!;
            PackagePartitionKey = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string PackagePartitionKey
        {
            get => _packagePartitionKey ??= Guid.NewGuid().ToString();
            set => _packagePartitionKey = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}