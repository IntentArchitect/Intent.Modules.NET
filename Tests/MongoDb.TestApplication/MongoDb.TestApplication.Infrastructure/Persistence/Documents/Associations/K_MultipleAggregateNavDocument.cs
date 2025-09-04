using System;
using System.Collections.Generic;
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
    internal class K_MultipleAggregateNavDocument : IK_MultipleAggregateNavDocument, IMongoDbDocument<K_MultipleAggregateNav, K_MultipleAggregateNavDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> JMultipledependentsIds { get; set; }

        public K_MultipleAggregateNav ToEntity(K_MultipleAggregateNav? entity = default)
        {
            entity ??= new K_MultipleAggregateNav();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JMultipledependentsIds = JMultipledependentsIds.ToList() ?? throw new Exception($"{nameof(entity.JMultipledependentsIds)} is null");

            return entity;
        }

        public K_MultipleAggregateNavDocument PopulateFromEntity(K_MultipleAggregateNav entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JMultipledependentsIds = entity.JMultipledependentsIds.ToList();

            return this;
        }

        public static K_MultipleAggregateNavDocument? FromEntity(K_MultipleAggregateNav? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new K_MultipleAggregateNavDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<K_MultipleAggregateNavDocument> GetIdFilter(string id)
        {
            return Builders<K_MultipleAggregateNavDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<K_MultipleAggregateNavDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<K_MultipleAggregateNavDocument> GetIdsFilter(string[] ids)
        {
            return Builders<K_MultipleAggregateNavDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<K_MultipleAggregateNavDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<K_MultipleAggregateNavDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}