using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class QuoteLine
    {
        public QuoteLine(Guid productId)
        {
            ProductId = productId;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected QuoteLine()
        {
            Product = null!;
        }
        public Guid Id { get; set; }

        public Guid QuoteId { get; set; }

        public Guid ProductId { get; set; }

        public int Units { get; set; }

        public decimal UnitPrice { get; set; }

        public virtual Product Product { get; set; }
    }
}