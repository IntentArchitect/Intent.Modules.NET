using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order()
        {
            RefNo = null!;
            Customer = null!;
        }
        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}