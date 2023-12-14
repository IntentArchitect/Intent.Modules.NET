using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class OrderItem : IOrderItem
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public int Quantity { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}