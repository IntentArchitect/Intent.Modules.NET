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
    internal abstract class BaseTypeOfTDocument<T> : IBaseTypeOfTDocument<T>, IMongoDbDocument<BaseTypeOfT<T>, BaseTypeOfTDocument<T>, string>
    {
        [BsonId]
        public string Id { get; set; }
        public T BaseAttribute { get; set; }

        public BaseTypeOfT<T> ToEntity(BaseTypeOfT<T>? entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.BaseAttribute = BaseAttribute ?? throw new Exception($"{nameof(entity.BaseAttribute)} is null");

            return entity;
        }

        public BaseTypeOfTDocument<T> PopulateFromEntity(BaseTypeOfT<T> entity)
        {
            Id = entity.Id;
            BaseAttribute = entity.BaseAttribute;

            return this;
        }

        public static FilterDefinition<BaseTypeOfTDocument<T>> GetIdFilter(string id)
        {
            return Builders<BaseTypeOfTDocument<T>>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<BaseTypeOfTDocument<T>> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<BaseTypeOfTDocument<T>> GetIdsFilter(string[] ids)
        {
            return Builders<BaseTypeOfTDocument<T>>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<BaseTypeOfTDocument<T>, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<BaseTypeOfTDocument<T>, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}