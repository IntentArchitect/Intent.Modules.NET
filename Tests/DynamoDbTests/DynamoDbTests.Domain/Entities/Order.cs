using DynamoDbTests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        private string? _id;
        private string? _warehouseId;
        public Order()
        {
            Id = null!;
            WarehouseId = null!;
            RefNo = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string WarehouseId
        {
            get => _warehouseId ??= Guid.NewGuid().ToString();
            set => _warehouseId = value;
        }

        public string RefNo { get; set; }

        public DateTime OrderDate { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}