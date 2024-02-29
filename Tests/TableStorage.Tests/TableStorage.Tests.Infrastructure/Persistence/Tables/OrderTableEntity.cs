using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Entities;
using TableStorage.Tests.Domain.Repositories.TableEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageTableEntity", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure.Persistence.Tables
{
    internal class OrderTableEntity : ITableAdapter<Order, OrderTableEntity>, IOrderTableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public string OrderNo { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Customer { get; set; } = default!;
        public string OrderLines { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            entity.PartitionKey = PartitionKey;
            entity.RowKey = RowKey;
            entity.OrderNo = OrderNo;
            entity.Amount = Amount;
            entity.Customer = JsonSerializer.Deserialize<Customer>(Customer);
            entity.OrderLines = JsonSerializer.Deserialize<ICollection<OrderLine>>(OrderLines);

            return entity;
        }

        public OrderTableEntity PopulateFromEntity(Order entity)
        {
            PartitionKey = entity.PartitionKey;
            RowKey = entity.RowKey;
            OrderNo = entity.OrderNo;
            Amount = entity.Amount;
            Customer = JsonSerializer.Serialize(entity.Customer);
            OrderLines = JsonSerializer.Serialize(entity.OrderLines);

            return this;
        }

        public static OrderTableEntity? FromEntity(Order? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderTableEntity().PopulateFromEntity(entity);
        }
    }
}