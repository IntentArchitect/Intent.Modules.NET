using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Collections;
using MongoDb.TestApplication.Domain.Repositories.Documents.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections
{
    internal class CustomCollectionEntityADocument : ICustomCollectionEntityADocument, IMongoDbDocument<CustomCollectionEntityA, CustomCollectionEntityADocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public CustomCollectionEntityA ToEntity(CustomCollectionEntityA? entity = default)
        {
            entity ??= new CustomCollectionEntityA();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public CustomCollectionEntityADocument PopulateFromEntity(CustomCollectionEntityA entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static CustomCollectionEntityADocument? FromEntity(CustomCollectionEntityA? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomCollectionEntityADocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CustomCollectionEntityADocument> GetIdFilter(string id)
        {
            return Builders<CustomCollectionEntityADocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CustomCollectionEntityADocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CustomCollectionEntityADocument> GetIdsFilter(string[] ids)
        {
            return Builders<CustomCollectionEntityADocument>.Filter.In(d => d.Id, ids);
        }
    }
}