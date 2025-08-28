using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Collections;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Collections;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Collections
{
    internal class CustomCollectionEntityBDocument : ICustomCollectionEntityBDocument, IMongoDbDocument<CustomCollectionEntityB, CustomCollectionEntityBDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public CustomCollectionEntityB ToEntity(CustomCollectionEntityB? entity = default)
        {
            entity ??= new CustomCollectionEntityB();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public CustomCollectionEntityBDocument PopulateFromEntity(CustomCollectionEntityB entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static CustomCollectionEntityBDocument? FromEntity(CustomCollectionEntityB? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomCollectionEntityBDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CustomCollectionEntityBDocument> GetIdFilter(string id)
        {
            return Builders<CustomCollectionEntityBDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CustomCollectionEntityBDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CustomCollectionEntityBDocument> GetIdsFilter(string[] ids)
        {
            return Builders<CustomCollectionEntityBDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<CustomCollectionEntityBDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<CustomCollectionEntityBDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}