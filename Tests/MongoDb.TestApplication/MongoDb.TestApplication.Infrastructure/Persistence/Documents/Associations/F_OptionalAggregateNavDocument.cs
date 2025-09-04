using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    internal class F_OptionalAggregateNavDocument : IF_OptionalAggregateNavDocument, IMongoDbDocument<F_OptionalAggregateNav, F_OptionalAggregateNavDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string? FOptionaldependentId { get; set; }

        public F_OptionalAggregateNav ToEntity(F_OptionalAggregateNav? entity = default)
        {
            entity ??= new F_OptionalAggregateNav();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.FOptionaldependentId = FOptionaldependentId;

            return entity;
        }

        public F_OptionalAggregateNavDocument PopulateFromEntity(F_OptionalAggregateNav entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            FOptionaldependentId = entity.FOptionaldependentId;

            return this;
        }

        public static F_OptionalAggregateNavDocument? FromEntity(F_OptionalAggregateNav? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new F_OptionalAggregateNavDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<F_OptionalAggregateNavDocument> GetIdFilter(string id)
        {
            return Builders<F_OptionalAggregateNavDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<F_OptionalAggregateNavDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<F_OptionalAggregateNavDocument> GetIdsFilter(string[] ids)
        {
            return Builders<F_OptionalAggregateNavDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<F_OptionalAggregateNavDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<F_OptionalAggregateNavDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}