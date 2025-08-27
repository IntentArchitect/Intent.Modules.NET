using System;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes
{
    internal class SingleIndexEntitySingleParentDocument : ISingleIndexEntitySingleParentDocument, IMongoDbDocument<SingleIndexEntitySingleParent, SingleIndexEntitySingleParentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public ISingleIndexEntitySingleChildDocument SingleIndexEntitySingleChild { get; set; }

        public SingleIndexEntitySingleParent ToEntity(SingleIndexEntitySingleParent? entity = default)
        {
            entity ??= new SingleIndexEntitySingleParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.SingleIndexEntitySingleChild = (SingleIndexEntitySingleChild as SingleIndexEntitySingleChildDocument).ToEntity() ?? throw new Exception($"{nameof(entity.SingleIndexEntitySingleChild)} is null");

            return entity;
        }

        public SingleIndexEntitySingleParentDocument PopulateFromEntity(SingleIndexEntitySingleParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            SingleIndexEntitySingleChild = SingleIndexEntitySingleChildDocument.FromEntity(entity.SingleIndexEntitySingleChild)!;

            return this;
        }

        public static SingleIndexEntitySingleParentDocument? FromEntity(SingleIndexEntitySingleParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SingleIndexEntitySingleParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<SingleIndexEntitySingleParentDocument> GetIdFilter(string id)
        {
            return Builders<SingleIndexEntitySingleParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<SingleIndexEntitySingleParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<SingleIndexEntitySingleParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<SingleIndexEntitySingleParentDocument>.Filter.In(d => d.Id, ids);
        }
    }
}