using System;
using System.Text.Json;
using Azure;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageTableEntity", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure.Persistence.Tables
{
    internal class InvoiceTableEntity : ITableAdapter<Invoice, InvoiceTableEntity>
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTime IssuedData { get; set; }
        public string OrderPartitionKey { get; set; } = default!;
        public string OrderRowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= new Invoice();

            entity.PartitionKey = PartitionKey;
            entity.RowKey = RowKey;
            entity.IssuedData = IssuedData;
            entity.OrderPartitionKey = OrderPartitionKey;
            entity.OrderRowKey = OrderRowKey;

            return entity;
        }

        public InvoiceTableEntity PopulateFromEntity(Invoice entity)
        {
            PartitionKey = entity.PartitionKey;
            RowKey = entity.RowKey;
            IssuedData = entity.IssuedData;
            OrderPartitionKey = entity.OrderPartitionKey;
            OrderRowKey = entity.OrderRowKey;

            return this;
        }

        public static InvoiceTableEntity? FromEntity(Invoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceTableEntity().PopulateFromEntity(entity);
        }
    }
}