using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey
{
    public class ODataOrderLine
    {
        public ODataOrderLine()
        {
            ProductName = null!;
            ODataProduct = null!;
        }

        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public Guid OrderId { get; set; }

        public Guid ODataProductId { get; set; }

        public virtual ODataProduct ODataProduct { get; set; }
    }
}