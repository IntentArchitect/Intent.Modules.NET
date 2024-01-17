using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class OrderItemDocument : IOrderItemDocument
    {
        public string Id { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; } = default!;

        public OrderItem ToEntity(OrderItem? entity = default)
        {
            entity ??= new OrderItem();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Quantity = Quantity;
            entity.Amount = Amount;
            entity.ProductId = ProductId ?? throw new Exception($"{nameof(entity.ProductId)} is null");

            return entity;
        }

        public OrderItemDocument PopulateFromEntity(OrderItem entity)
        {
            Id = entity.Id;
            Quantity = entity.Quantity;
            Amount = entity.Amount;
            ProductId = entity.ProductId;

            return this;
        }

        public static OrderItemDocument? FromEntity(OrderItem? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderItemDocument().PopulateFromEntity(entity);
        }
    }
}