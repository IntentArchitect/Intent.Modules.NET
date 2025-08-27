using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents
{
    internal abstract class BaseTypeDocument : IBaseTypeDocument, IMongoDbDocument<BaseType, BaseTypeDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string BaseAttribute { get; set; }

        public BaseType ToEntity(BaseType? entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.BaseAttribute = BaseAttribute ?? throw new Exception($"{nameof(entity.BaseAttribute)} is null");

            return entity;
        }

        public BaseTypeDocument PopulateFromEntity(BaseType entity)
        {
            Id = entity.Id;
            BaseAttribute = entity.BaseAttribute;

            return this;
        }

        public static FilterDefinition<BaseTypeDocument> GetIdFilter(string id)
        {
            return Builders<BaseTypeDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<BaseTypeDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<BaseTypeDocument> GetIdsFilter(string[] ids)
        {
            return Builders<BaseTypeDocument>.Filter.In(d => d.Id, ids);
        }
    }
}