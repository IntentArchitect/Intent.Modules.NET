using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes
{
    internal class SingleIndexEntityDocument : ISingleIndexEntityDocument, IMongoDbDocument<SingleIndexEntity, SingleIndexEntityDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public string SingleIndex { get; set; }

        public SingleIndexEntity ToEntity(SingleIndexEntity? entity = default)
        {
            entity ??= new SingleIndexEntity();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.SingleIndex = SingleIndex ?? throw new Exception($"{nameof(entity.SingleIndex)} is null");

            return entity;
        }

        public SingleIndexEntityDocument PopulateFromEntity(SingleIndexEntity entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            SingleIndex = entity.SingleIndex;

            return this;
        }

        public static SingleIndexEntityDocument? FromEntity(SingleIndexEntity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SingleIndexEntityDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<SingleIndexEntityDocument> GetIdFilter(string id)
        {
            return Builders<SingleIndexEntityDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<SingleIndexEntityDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<SingleIndexEntityDocument> GetIdsFilter(string[] ids)
        {
            return Builders<SingleIndexEntityDocument>.Filter.In(d => d.Id, ids);
        }
    }
}