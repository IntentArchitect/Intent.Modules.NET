using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.NestedAssociations;
using MongoDb.TestApplication.Domain.Repositories.Documents.NestedAssociations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.NestedAssociations
{
    [BsonDiscriminator(nameof(NestedCompositionA), Required = true)]
    internal class NestedCompositionADocument : INestedCompositionADocument
    {
        public string Attribute { get; set; } = default!;
        public string AggregateBId { get; set; } = default!;

        public NestedCompositionA ToEntity(NestedCompositionA? entity = default)
        {
            entity ??= new NestedCompositionA();

            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.AggregateBId = AggregateBId ?? throw new Exception($"{nameof(entity.AggregateBId)} is null");

            return entity;
        }

        public NestedCompositionADocument PopulateFromEntity(NestedCompositionA entity)
        {
            Attribute = entity.Attribute;
            AggregateBId = entity.AggregateBId;

            return this;
        }

        public static NestedCompositionADocument? FromEntity(NestedCompositionA? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new NestedCompositionADocument().PopulateFromEntity(entity);
        }
    }
}