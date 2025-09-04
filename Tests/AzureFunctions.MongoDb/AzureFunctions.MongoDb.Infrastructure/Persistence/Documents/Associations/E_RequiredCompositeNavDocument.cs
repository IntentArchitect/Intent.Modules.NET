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
    internal class E_RequiredCompositeNavDocument : IE_RequiredCompositeNavDocument, IMongoDbDocument<E_RequiredCompositeNav, E_RequiredCompositeNavDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IE_RequiredDependentDocument E_RequiredDependent { get; set; }

        public E_RequiredCompositeNav ToEntity(E_RequiredCompositeNav? entity = default)
        {
            entity ??= new E_RequiredCompositeNav();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.E_RequiredDependent = (E_RequiredDependent as E_RequiredDependentDocument).ToEntity() ?? throw new Exception($"{nameof(entity.E_RequiredDependent)} is null");

            return entity;
        }

        public E_RequiredCompositeNavDocument PopulateFromEntity(E_RequiredCompositeNav entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            E_RequiredDependent = E_RequiredDependentDocument.FromEntity(entity.E_RequiredDependent)!;

            return this;
        }

        public static E_RequiredCompositeNavDocument? FromEntity(E_RequiredCompositeNav? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new E_RequiredCompositeNavDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<E_RequiredCompositeNavDocument> GetIdFilter(string id)
        {
            return Builders<E_RequiredCompositeNavDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<E_RequiredCompositeNavDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<E_RequiredCompositeNavDocument> GetIdsFilter(string[] ids)
        {
            return Builders<E_RequiredCompositeNavDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<E_RequiredCompositeNavDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<E_RequiredCompositeNavDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}