using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        private string? _id;
        private string? _warehouseId;

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

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}