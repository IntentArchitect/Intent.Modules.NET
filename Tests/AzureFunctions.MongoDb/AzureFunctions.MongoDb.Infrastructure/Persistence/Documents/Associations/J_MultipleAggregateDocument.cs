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
    [BsonDiscriminator(nameof(J_MultipleAggregate), Required = true)]
    internal class J_MultipleAggregateDocument : IJ_MultipleAggregateDocument, IMongoDbDocument<J_MultipleAggregate, J_MultipleAggregateDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> JMultipleDependentsIds { get; set; }

        public J_MultipleAggregate ToEntity(J_MultipleAggregate? entity = default)
        {
            entity ??= new J_MultipleAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JMultipleDependentsIds = JMultipleDependentsIds.ToList() ?? throw new Exception($"{nameof(entity.JMultipleDependentsIds)} is null");

            return entity;
        }

        public J_MultipleAggregateDocument PopulateFromEntity(J_MultipleAggregate entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JMultipleDependentsIds = entity.JMultipleDependentsIds.ToList();

            return this;
        }

        public static J_MultipleAggregateDocument? FromEntity(J_MultipleAggregate? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new J_MultipleAggregateDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<J_MultipleAggregateDocument> GetIdFilter(string id)
        {
            return Builders<J_MultipleAggregateDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<J_MultipleAggregateDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<J_MultipleAggregateDocument> GetIdsFilter(string[] ids)
        {
            return Builders<J_MultipleAggregateDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<J_MultipleAggregateDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<J_MultipleAggregateDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}