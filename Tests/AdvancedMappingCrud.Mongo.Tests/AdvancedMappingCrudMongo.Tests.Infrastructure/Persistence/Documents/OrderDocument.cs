using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents
{
    internal class OrderDocument : IOrderDocument, IMongoDbDocument<Order, OrderDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RefNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ExternalRef { get; set; }
        public IEnumerable<IOrderItemDocument> OrderItems { get; set; }

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CustomerId = CustomerId ?? throw new Exception($"{nameof(entity.CustomerId)} is null");
            entity.RefNo = RefNo ?? throw new Exception($"{nameof(entity.RefNo)} is null");
            entity.OrderDate = OrderDate;
            entity.ExternalRef = ExternalRef ?? throw new Exception($"{nameof(entity.ExternalRef)} is null");
            entity.OrderItems = OrderItems.Select(x => (x as OrderItemDocument).ToEntity()).ToList();

            return entity;
        }

        public OrderDocument PopulateFromEntity(Order entity)
        {
            Id = entity.Id;
            CustomerId = entity.CustomerId;
            RefNo = entity.RefNo;
            OrderDate = entity.OrderDate;
            ExternalRef = entity.ExternalRef;
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

        public static FilterDefinition<OrderDocument> GetIdFilter(string id)
        {
            return Builders<OrderDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<OrderDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<OrderDocument> GetIdsFilter(string[] ids)
        {
            return Builders<OrderDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<OrderDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<OrderDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}