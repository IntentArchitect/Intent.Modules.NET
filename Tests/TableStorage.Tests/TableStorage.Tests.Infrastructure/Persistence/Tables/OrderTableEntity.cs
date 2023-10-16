using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageTableEntity", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure.Persistence.Tables
{
    internal class OrderTableEntity : ITableAdapter<Order, OrderTableEntity>
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public string OrderNo { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Customer { get; set; } = default!;
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

            return entity;
        }

        public OrderTableEntity PopulateFromEntity(Order entity)
        {
            PartitionKey = entity.PartitionKey;
            RowKey = entity.RowKey;
            OrderNo = entity.OrderNo;
            Amount = entity.Amount;
            Customer = JsonSerializer.Serialize(entity.Customer);

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