using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Domain.Entities
{
    public class OrderLine
    {
        public OrderLine()
        {
            Product = null!;
        }
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Units { get; set; }

        public decimal UnitPrice { get; set; }

        public IList<decimal> Discount { get; set; } = [];

        public virtual Product Product { get; set; }
    }
}