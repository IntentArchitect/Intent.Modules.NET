using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string CustomerId { get; set; }

        public string RefNo { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<OrderTags> OrderTags { get; set; } = new List<OrderTags>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}