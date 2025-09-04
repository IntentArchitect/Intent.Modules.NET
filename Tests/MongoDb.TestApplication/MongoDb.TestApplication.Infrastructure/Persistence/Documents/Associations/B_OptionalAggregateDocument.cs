using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    internal class B_OptionalAggregateDocument : IB_OptionalAggregateDocument, IMongoDbDocument<B_OptionalAggregate, B_OptionalAggregateDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string? BOptionaldependentId { get; set; }

        public B_OptionalAggregate ToEntity(B_OptionalAggregate? entity = default)
        {
            entity ??= new B_OptionalAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.BOptionaldependentId = BOptionaldependentId;

            return entity;
        }

        public B_OptionalAggregateDocument PopulateFromEntity(B_OptionalAggregate entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            BOptionaldependentId = entity.BOptionaldependentId;

            return this;
        }

        public static B_OptionalAggregateDocument? FromEntity(B_OptionalAggregate? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new B_OptionalAggregateDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<B_OptionalAggregateDocument> GetIdFilter(string id)
        {
            return Builders<B_OptionalAggregateDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<B_OptionalAggregateDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<B_OptionalAggregateDocument> GetIdsFilter(string[] ids)
        {
            return Builders<B_OptionalAggregateDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<B_OptionalAggregateDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<B_OptionalAggregateDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}