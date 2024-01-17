using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities
{
    public class OrderItem
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public string ProductId { get; set; }
    }
}