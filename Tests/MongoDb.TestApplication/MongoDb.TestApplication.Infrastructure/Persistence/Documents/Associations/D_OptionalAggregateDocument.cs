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
    internal class D_OptionalAggregateDocument : ID_OptionalAggregateDocument, IMongoDbDocument<D_OptionalAggregate, D_OptionalAggregateDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public D_OptionalAggregate ToEntity(D_OptionalAggregate? entity = default)
        {
            entity ??= new D_OptionalAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public D_OptionalAggregateDocument PopulateFromEntity(D_OptionalAggregate entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static D_OptionalAggregateDocument? FromEntity(D_OptionalAggregate? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new D_OptionalAggregateDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<D_OptionalAggregateDocument> GetIdFilter(string id)
        {
            return Builders<D_OptionalAggregateDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<D_OptionalAggregateDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<D_OptionalAggregateDocument> GetIdsFilter(string[] ids)
        {
            return Builders<D_OptionalAggregateDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<D_OptionalAggregateDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<D_OptionalAggregateDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}