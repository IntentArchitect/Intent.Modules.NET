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
    [BsonDiscriminator(nameof(K_MultipleDependent), Required = true)]
    internal class K_MultipleDependentDocument : IK_MultipleDependentDocument, IMongoDbDocument<K_MultipleDependent, K_MultipleDependentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> JMultipleAggregatesIds { get; set; }

        public K_MultipleDependent ToEntity(K_MultipleDependent? entity = default)
        {
            entity ??= new K_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JMultipleAggregatesIds = JMultipleAggregatesIds.ToList() ?? throw new Exception($"{nameof(entity.JMultipleAggregatesIds)} is null");

            return entity;
        }

        public K_MultipleDependentDocument PopulateFromEntity(K_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JMultipleAggregatesIds = entity.JMultipleAggregatesIds.ToList();

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