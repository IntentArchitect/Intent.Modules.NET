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
    internal class DerivedOfTDocument : BaseTypeOfTDocument<int>, IDerivedOfTDocument, IMongoDbDocument<DerivedOfT, DerivedOfTDocument, string>
    {
        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(DerivedOfT entity)
        {
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