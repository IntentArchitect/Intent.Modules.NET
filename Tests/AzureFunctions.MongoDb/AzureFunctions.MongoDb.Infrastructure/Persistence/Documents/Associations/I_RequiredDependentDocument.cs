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
    internal class I_RequiredDependentDocument : II_RequiredDependentDocument, IMongoDbDocument<I_RequiredDependent, I_RequiredDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public I_RequiredDependent ToEntity(I_RequiredDependent? entity = default)
        {
            entity ??= new I_RequiredDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public I_RequiredDependentDocument PopulateFromEntity(I_RequiredDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static I_RequiredDependentDocument? FromEntity(I_RequiredDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new I_RequiredDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<I_RequiredDependentDocument> GetIdFilter(string id)
        {
            return Builders<I_RequiredDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<I_RequiredDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<I_RequiredDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<I_RequiredDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<I_RequiredDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<I_RequiredDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}