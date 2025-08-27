using System;
using AzureFunctions.MongoDb.Domain.Entities.NestedAssociations;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.NestedAssociations
{
    internal class AggregateADocument : IAggregateADocument, IMongoDbDocument<AggregateA, AggregateADocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public INestedCompositionADocument NestedCompositionA { get; set; }

        public AggregateA ToEntity(AggregateA? entity = default)
        {
            entity ??= new AggregateA();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.NestedCompositionA = (NestedCompositionA as NestedCompositionADocument).ToEntity() ?? throw new Exception($"{nameof(entity.NestedCompositionA)} is null");

            return entity;
        }

        public AggregateADocument PopulateFromEntity(AggregateA entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            NestedCompositionA = NestedCompositionADocument.FromEntity(entity.NestedCompositionA)!;

            return this;
        }

        public static AggregateADocument? FromEntity(AggregateA? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AggregateADocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<AggregateADocument> GetIdFilter(string id)
        {
            return Builders<AggregateADocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<AggregateADocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<AggregateADocument> GetIdsFilter(string[] ids)
        {
            return Builders<AggregateADocument>.Filter.In(d => d.Id, ids);
        }
    }
}