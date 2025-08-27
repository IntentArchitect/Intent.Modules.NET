using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.IdTypes;
using MongoDb.TestApplication.Domain.Repositories.Documents.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.IdTypes
{
    internal class IdTypeOjectIdStrDocument : IIdTypeOjectIdStrDocument, IMongoDbDocument<IdTypeOjectIdStr, IdTypeOjectIdStrDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public IdTypeOjectIdStr ToEntity(IdTypeOjectIdStr? entity = default)
        {
            entity ??= new IdTypeOjectIdStr();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public IdTypeOjectIdStrDocument PopulateFromEntity(IdTypeOjectIdStr entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static IdTypeOjectIdStrDocument? FromEntity(IdTypeOjectIdStr? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new IdTypeOjectIdStrDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<IdTypeOjectIdStrDocument> GetIdFilter(string id)
        {
            return Builders<IdTypeOjectIdStrDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<IdTypeOjectIdStrDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<IdTypeOjectIdStrDocument> GetIdsFilter(string[] ids)
        {
            return Builders<IdTypeOjectIdStrDocument>.Filter.In(d => d.Id, ids);
        }
    }
}