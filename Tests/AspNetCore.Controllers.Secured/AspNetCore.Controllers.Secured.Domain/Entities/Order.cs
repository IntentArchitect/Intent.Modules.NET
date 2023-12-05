using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid BuyerId { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual Buyer Buyer { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}