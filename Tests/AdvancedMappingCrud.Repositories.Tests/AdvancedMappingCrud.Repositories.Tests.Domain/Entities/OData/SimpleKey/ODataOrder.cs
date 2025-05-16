using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey
{
    public class ODataOrder
    {
        public ODataOrder()
        {
            Description = null!;
            DateOfOrder = null!;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public string DateOfOrder { get; set; }

        public Guid CustomerId { get; set; }

        public virtual ICollection<ODataOrderLine> ODataOrderLines { get; set; } = [];
    }
}