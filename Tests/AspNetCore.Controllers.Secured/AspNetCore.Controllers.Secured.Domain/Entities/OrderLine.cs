using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace AspNetCore.Controllers.Secured.Domain.Entities
{
    public class OrderLine
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Units { get; set; }

        public decimal UnitPrice { get; set; }

        public ICollection<decimal> Discount { get; set; } = new List<decimal>();

        public virtual Product Product { get; set; }
    }
}