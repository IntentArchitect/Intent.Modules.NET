using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    internal class I_MultipleAggregateDocument : II_MultipleAggregateDocument, IMongoDbDocument<I_MultipleAggregate, I_MultipleAggregateDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string JRequireddependentId { get; set; }

        public I_MultipleAggregate ToEntity(I_MultipleAggregate? entity = default)
        {
            entity ??= new I_MultipleAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JRequireddependentId = JRequireddependentId ?? throw new Exception($"{nameof(entity.JRequireddependentId)} is null");

            return entity;
        }

        public I_MultipleAggregateDocument PopulateFromEntity(I_MultipleAggregate entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JRequireddependentId = entity.JRequireddependentId;

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
    }
}