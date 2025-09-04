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
    internal class A_RequiredCompositeDocument : IA_RequiredCompositeDocument, IMongoDbDocument<A_RequiredComposite, A_RequiredCompositeDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string ReqCompAttribute { get; set; }
        public IA_OptionalDependentDocument? A_OptionalDependent { get; set; }

        public A_RequiredComposite ToEntity(A_RequiredComposite? entity = default)
        {
            entity ??= new A_RequiredComposite();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ReqCompAttribute = ReqCompAttribute ?? throw new Exception($"{nameof(entity.ReqCompAttribute)} is null");
            entity.A_OptionalDependent = (A_OptionalDependent as A_OptionalDependentDocument)?.ToEntity();

            return entity;
        }

        public A_RequiredCompositeDocument PopulateFromEntity(A_RequiredComposite entity)
        {
            Id = entity.Id;
            ReqCompAttribute = entity.ReqCompAttribute;
            A_OptionalDependent = A_OptionalDependentDocument.FromEntity(entity.A_OptionalDependent);

            return this;
        }

        public static A_RequiredCompositeDocument? FromEntity(A_RequiredComposite? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new A_RequiredCompositeDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<A_RequiredCompositeDocument> GetIdFilter(string id)
        {
            return Builders<A_RequiredCompositeDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<A_RequiredCompositeDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<A_RequiredCompositeDocument> GetIdsFilter(string[] ids)
        {
            return Builders<A_RequiredCompositeDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<A_RequiredCompositeDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<A_RequiredCompositeDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}