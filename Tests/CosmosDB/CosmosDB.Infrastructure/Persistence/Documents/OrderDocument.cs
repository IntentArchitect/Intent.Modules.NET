using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class OrderDocument : IOrderDocument, ICosmosDBDocument<Order, OrderDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
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
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string WarehouseId { get; set; } = default!;
        public string RefNo { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public List<OrderItemDocument> OrderItems { get; set; } = default!;
        IReadOnlyList<IOrderItemDocument> IOrderDocument.OrderItems => OrderItems;

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

        public OrderDocument PopulateFromEntity(Order entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            WarehouseId = entity.WarehouseId;
            RefNo = entity.RefNo;
            OrderDate = entity.OrderDate;
            OrderItems = entity.OrderItems.Select(x => OrderItemDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static OrderDocument? FromEntity(Order? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}