using DynamoDbTests.EntityInterfaces.Domain.Common;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class Order : IOrder, IHasDomainEvent
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

        ICollection<IOrderItem> IOrder.OrderItems
        {
            get => OrderItems.CreateWrapper<IOrderItem, OrderItem>();
            set => OrderItems = value.Cast<OrderItem>().ToList();
        }
    }
}