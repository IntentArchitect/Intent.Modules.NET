using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
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
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public string RefNo { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderItemDocument> OrderItems { get; set; } = default!;
        IReadOnlyList<IOrderItemDocument> IOrderDocument.OrderItems => OrderItems;
        public List<OrderTagsDocument> OrderTags { get; set; } = default!;
        IReadOnlyList<IOrderTagsDocument> IOrderDocument.OrderTags => OrderTags;

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CustomerId = CustomerId ?? throw new Exception($"{nameof(entity.CustomerId)} is null");
            entity.RefNo = RefNo ?? throw new Exception($"{nameof(entity.RefNo)} is null");
            entity.OrderDate = OrderDate;
            entity.OrderStatus = OrderStatus;
            entity.OrderItems = OrderItems.Select(x => x.ToEntity()).ToList();
            entity.OrderTags = OrderTags.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public OrderDocument PopulateFromEntity(Order entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            CustomerId = entity.CustomerId;
            RefNo = entity.RefNo;
            OrderDate = entity.OrderDate;
            OrderStatus = entity.OrderStatus;
            OrderItems = entity.OrderItems.Select(x => OrderItemDocument.FromEntity(x)!).ToList();
            OrderTags = entity.OrderTags.Select(x => OrderTagsDocument.FromEntity(x)!).ToList();

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