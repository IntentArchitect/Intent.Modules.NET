using System;
using System.Collections.Generic;
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
    internal class H_OptionalAggregateNavDocument : IH_OptionalAggregateNavDocument, IMongoDbDocument<H_OptionalAggregateNav, H_OptionalAggregateNavDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> HMultipleDependentsIds { get; set; }

        public H_OptionalAggregateNav ToEntity(H_OptionalAggregateNav? entity = default)
        {
            entity ??= new H_OptionalAggregateNav();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.HMultipleDependentsIds = HMultipleDependentsIds.ToList() ?? throw new Exception($"{nameof(entity.HMultipleDependentsIds)} is null");

            return entity;
        }

        public H_OptionalAggregateNavDocument PopulateFromEntity(H_OptionalAggregateNav entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            HMultipleDependentsIds = entity.HMultipleDependentsIds.ToList();

            return this;
        }

        public static H_OptionalAggregateNavDocument? FromEntity(H_OptionalAggregateNav? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new H_OptionalAggregateNavDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<H_OptionalAggregateNavDocument> GetIdFilter(string id)
        {
            return Builders<H_OptionalAggregateNavDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<H_OptionalAggregateNavDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<H_OptionalAggregateNavDocument> GetIdsFilter(string[] ids)
        {
            return Builders<H_OptionalAggregateNavDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<H_OptionalAggregateNavDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<H_OptionalAggregateNavDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}