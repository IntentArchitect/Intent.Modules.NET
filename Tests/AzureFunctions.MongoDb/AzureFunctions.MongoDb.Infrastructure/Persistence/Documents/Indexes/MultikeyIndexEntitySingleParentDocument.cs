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
    internal class MultikeyIndexEntitySingleParentDocument : IMultikeyIndexEntitySingleParentDocument, IMongoDbDocument<MultikeyIndexEntitySingleParent, MultikeyIndexEntitySingleParentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public IMultikeyIndexEntitySingleChildDocument MultikeyIndexEntitySingleChild { get; set; }

        public MultikeyIndexEntitySingleParent ToEntity(MultikeyIndexEntitySingleParent? entity = default)
        {
            entity ??= new MultikeyIndexEntitySingleParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.MultikeyIndexEntitySingleChild = (MultikeyIndexEntitySingleChild as MultikeyIndexEntitySingleChildDocument).ToEntity() ?? throw new Exception($"{nameof(entity.MultikeyIndexEntitySingleChild)} is null");

            return entity;
        }

        public MultikeyIndexEntitySingleParentDocument PopulateFromEntity(MultikeyIndexEntitySingleParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            MultikeyIndexEntitySingleChild = MultikeyIndexEntitySingleChildDocument.FromEntity(entity.MultikeyIndexEntitySingleChild)!;

            return this;
        }

        public static MultikeyIndexEntitySingleParentDocument? FromEntity(MultikeyIndexEntitySingleParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MultikeyIndexEntitySingleParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MultikeyIndexEntitySingleParentDocument> GetIdFilter(string id)
        {
            return Builders<MultikeyIndexEntitySingleParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MultikeyIndexEntitySingleParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MultikeyIndexEntitySingleParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MultikeyIndexEntitySingleParentDocument>.Filter.In(d => d.Id, ids);
        }
    }
}