using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("orders")]
    internal class OrderDocument : IDynamoDBDocument<Order, OrderDocument>
    {
        [DynamoDBRangeKey]
        public string Id { get; set; } = default!;
        [DynamoDBHashKey]
        public string WarehouseId { get; set; } = default!;
        public string RefNo { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        [DynamoDBVersion]
        public int? Version { get; set; }
        public List<OrderItemDocument> OrderItems { get; set; } = default!;

        public object GetKey() => (WarehouseId, Id);

        public int? GetVersion() => Version;

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.WarehouseId = WarehouseId ?? throw new Exception($"{nameof(entity.WarehouseId)} is null");
            entity.RefNo = RefNo ?? throw new Exception($"{nameof(entity.RefNo)} is null");
            entity.OrderDate = OrderDate;
            entity.OrderItems = OrderItems.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public OrderDocument PopulateFromEntity(Order entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            WarehouseId = entity.WarehouseId;
            RefNo = entity.RefNo;
            OrderDate = entity.OrderDate;
            OrderItems = entity.OrderItems.Select(x => OrderItemDocument.FromEntity(x)!).ToList();
            Version ??= getVersion(GetKey());

            return this;
        }

        public static OrderDocument? FromEntity(Order? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}