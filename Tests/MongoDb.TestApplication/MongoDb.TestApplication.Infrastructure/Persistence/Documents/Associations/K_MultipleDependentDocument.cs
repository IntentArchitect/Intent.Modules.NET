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
    internal class K_MultipleDependentDocument : IK_MultipleDependentDocument, IMongoDbDocument<K_MultipleDependent, K_MultipleDependentDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> JMultipleaggregatesIds { get; set; }

        public K_MultipleDependent ToEntity(K_MultipleDependent? entity = default)
        {
            entity ??= new K_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JMultipleaggregatesIds = JMultipleaggregatesIds.ToList() ?? throw new Exception($"{nameof(entity.JMultipleaggregatesIds)} is null");

            return entity;
        }

        public K_MultipleDependentDocument PopulateFromEntity(K_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JMultipleaggregatesIds = entity.JMultipleaggregatesIds.ToList();

            return this;
        }

        public static K_MultipleDependentDocument? FromEntity(K_MultipleDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new K_MultipleDependentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<K_MultipleDependentDocument> GetIdFilter(string id)
        {
            return Builders<K_MultipleDependentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<K_MultipleDependentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<K_MultipleDependentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<K_MultipleDependentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<K_MultipleDependentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<K_MultipleDependentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}