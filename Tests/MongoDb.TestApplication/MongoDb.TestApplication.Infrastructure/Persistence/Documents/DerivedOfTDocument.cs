using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents
{
    [BsonDiscriminator(nameof(DerivedOfT), Required = true)]
    internal class DerivedOfTDocument : BaseTypeOfTDocument<int>, IDerivedOfTDocument, IMongoDbDocument<DerivedOfT, DerivedOfTDocument, string>
    {
        public string DerivedAttribute { get; set; }

        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();

            entity.DerivedAttribute = DerivedAttribute ?? throw new Exception($"{nameof(entity.DerivedAttribute)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(DerivedOfT entity)
        {
            DerivedAttribute = entity.DerivedAttribute;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedOfTDocument? FromEntity(DerivedOfT? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedOfTDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<DerivedOfTDocument> GetIdFilter(string id)
        {
            return Builders<DerivedOfTDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<DerivedOfTDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<DerivedOfTDocument> GetIdsFilter(string[] ids)
        {
            return Builders<DerivedOfTDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<DerivedOfTDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<DerivedOfTDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}