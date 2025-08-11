using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class OrderItemDocument
    {
        public string Id { get; set; } = default!;
        public int Quantity { get; set; }
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }

        public OrderItem ToEntity(OrderItem? entity = default)
        {
            entity ??= new OrderItem();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Quantity = Quantity;
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Amount = Amount;

            return entity;
        }

        public OrderItemDocument PopulateFromEntity(IOrderItem entity)
        {
            Id = entity.Id;
            Quantity = entity.Quantity;
            Description = entity.Description;
            Amount = entity.Amount;

            return this;
        }

        public static OrderItemDocument? FromEntity(IOrderItem? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderItemDocument().PopulateFromEntity(entity);
        }
    }
}