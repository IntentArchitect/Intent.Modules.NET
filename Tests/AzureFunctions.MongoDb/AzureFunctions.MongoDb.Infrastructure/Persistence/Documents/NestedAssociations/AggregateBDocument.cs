using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.NestedAssociations;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.NestedAssociations
{
    [BsonDiscriminator(nameof(AggregateB), Required = true)]
    internal class AggregateBDocument : IAggregateBDocument, IMongoDbDocument<AggregateB, AggregateBDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public AggregateB ToEntity(AggregateB? entity = default)
        {
            entity ??= new AggregateB();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public AggregateBDocument PopulateFromEntity(AggregateB entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static AggregateBDocument? FromEntity(AggregateB? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AggregateBDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<AggregateBDocument> GetIdFilter(string id)
        {
            return Builders<AggregateBDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<AggregateBDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<AggregateBDocument> GetIdsFilter(string[] ids)
        {
            return Builders<AggregateBDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<AggregateBDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<AggregateBDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}