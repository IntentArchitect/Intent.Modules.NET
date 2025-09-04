using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Associations
{
    [BsonDiscriminator(nameof(I_MultipleAggregate), Required = true)]
    internal class I_MultipleAggregateDocument : II_MultipleAggregateDocument, IMongoDbDocument<I_MultipleAggregate, I_MultipleAggregateDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string JRequiredDependentId { get; set; }

        public I_MultipleAggregate ToEntity(I_MultipleAggregate? entity = default)
        {
            entity ??= new I_MultipleAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JRequiredDependentId = JRequiredDependentId ?? throw new Exception($"{nameof(entity.JRequiredDependentId)} is null");

            return entity;
        }

        public I_MultipleAggregateDocument PopulateFromEntity(I_MultipleAggregate entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JRequiredDependentId = entity.JRequiredDependentId;

            return this;
        }

        public static I_MultipleAggregateDocument? FromEntity(I_MultipleAggregate? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new I_MultipleAggregateDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<I_MultipleAggregateDocument> GetIdFilter(string id)
        {
            return Builders<I_MultipleAggregateDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<I_MultipleAggregateDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<I_MultipleAggregateDocument> GetIdsFilter(string[] ids)
        {
            return Builders<I_MultipleAggregateDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<I_MultipleAggregateDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<I_MultipleAggregateDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}