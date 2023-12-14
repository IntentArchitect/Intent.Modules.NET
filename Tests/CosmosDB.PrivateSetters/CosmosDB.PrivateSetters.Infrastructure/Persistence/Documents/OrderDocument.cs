using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class OrderDocument : IOrderDocument, ICosmosDBDocument<Order, OrderDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? ICosmosDBDocument.PartitionKey
        {
            get => WarehouseId;
            set => WarehouseId = value!;
        }
        public string Id { get; set; } = default!;
        public string WarehouseId { get; set; } = default!;
        public string RefNo { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public List<OrderItemDocument> OrderItems { get; set; } = default!;
        IReadOnlyList<IOrderItemDocument> IOrderDocument.OrderItems => OrderItems;

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(WarehouseId), WarehouseId ?? throw new Exception($"{nameof(entity.WarehouseId)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(RefNo), RefNo ?? throw new Exception($"{nameof(entity.RefNo)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(OrderDate), OrderDate);
            ReflectionHelper.ForceSetProperty(entity, nameof(OrderItems), OrderItems.Select(x => x.ToEntity()).ToList());

            return entity;
        }

        public OrderDocument PopulateFromEntity(Order entity)
        {
            Id = entity.Id;
            WarehouseId = entity.WarehouseId;
            RefNo = entity.RefNo;
            OrderDate = entity.OrderDate;
            OrderItems = entity.OrderItems.Select(x => OrderItemDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static OrderDocument? FromEntity(Order? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderDocument().PopulateFromEntity(entity);
        }
    }
}