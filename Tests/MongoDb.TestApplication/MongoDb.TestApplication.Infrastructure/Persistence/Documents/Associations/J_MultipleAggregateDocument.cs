using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    internal class J_MultipleAggregateDocument : IJ_MultipleAggregateDocument, IMongoDbDocument<J_MultipleAggregate, J_MultipleAggregateDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> JMultipledependentsIds { get; set; }

        public J_MultipleAggregate ToEntity(J_MultipleAggregate? entity = default)
        {
            entity ??= new J_MultipleAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.JMultipledependentsIds = JMultipledependentsIds.ToList() ?? throw new Exception($"{nameof(entity.JMultipledependentsIds)} is null");

            return entity;
        }

        public J_MultipleAggregateDocument PopulateFromEntity(J_MultipleAggregate entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            JMultipledependentsIds = entity.JMultipledependentsIds.ToList();

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
    }
}