using System;
using System.Collections.Generic;
using IntegrationTesting.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class RichProduct : IHasDomainEvent
    {
        public RichProduct(Guid brandId, string name)
        {
            BrandId = brandId;
            Name = name;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected RichProduct()
        {
            Name = null!;
            Brand = null!;
        }

        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public string Name { get; set; }

        public virtual Brand Brand { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}