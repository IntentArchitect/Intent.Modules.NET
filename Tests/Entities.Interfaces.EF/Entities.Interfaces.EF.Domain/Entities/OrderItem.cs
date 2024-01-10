using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.Interfaces.EF.Domain.Entities
{
    public class OrderItem : IOrderItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid OrderId { get; set; }
    }
}