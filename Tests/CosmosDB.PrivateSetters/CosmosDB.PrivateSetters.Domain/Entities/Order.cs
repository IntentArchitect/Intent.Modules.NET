using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        private List<OrderItem> _orderItems = new List<OrderItem>();
        private string? _id;
        private string? _warehouseId;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string WarehouseId
        {
            get => _warehouseId ??= Guid.NewGuid().ToString();
            private set => _warehouseId = value;
        }

        public string RefNo { get; private set; }

        public DateTime OrderDate { get; private set; }

        public IReadOnlyCollection<OrderItem> OrderItems
        {
            get => _orderItems.AsReadOnly();
            private set => _orderItems = new List<OrderItem>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}