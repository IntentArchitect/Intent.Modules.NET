using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities;
using AzureFunctions.MongoDb.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents
{
    [BsonDiscriminator(nameof(Derived), Required = true)]
    internal class DerivedDocument : BaseTypeDocument, IDerivedDocument, IMongoDbDocument<Derived, DerivedDocument, string>
    {
        public string DerivedAttribute { get; set; }

        public Derived ToEntity(Derived? entity = default)
        {
            entity ??= new Derived();

            entity.DerivedAttribute = DerivedAttribute ?? throw new Exception($"{nameof(entity.DerivedAttribute)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedDocument PopulateFromEntity(Derived entity)
        {
            DerivedAttribute = entity.DerivedAttribute;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedDocument? FromEntity(Derived? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<DerivedDocument> GetIdFilter(string id)
        {
            return Builders<DerivedDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<DerivedDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<DerivedDocument> GetIdsFilter(string[] ids)
        {
            return Builders<DerivedDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<DerivedDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<DerivedDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}