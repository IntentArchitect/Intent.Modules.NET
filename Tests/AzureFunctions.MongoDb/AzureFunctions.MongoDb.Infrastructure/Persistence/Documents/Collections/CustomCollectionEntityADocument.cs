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
    internal class CustomCollectionEntityADocument : ICustomCollectionEntityADocument, IMongoDbDocument<CustomCollectionEntityA, CustomCollectionEntityADocument, string>
    {
        [BsonId]
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

        public static Expression<Func<CustomCollectionEntityADocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<CustomCollectionEntityADocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}