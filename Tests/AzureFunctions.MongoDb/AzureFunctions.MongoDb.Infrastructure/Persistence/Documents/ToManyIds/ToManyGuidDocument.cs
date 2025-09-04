using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.ToManyIds
{
    [BsonDiscriminator(nameof(ToManyGuid), Required = true)]
    internal class ToManyGuidDocument : IToManyGuidDocument
    {
        public Guid Id { get; set; }

        public ToManyGuid ToEntity(ToManyGuid? entity = default)
        {
            entity ??= new ToManyGuid();

            entity.Id = Id;

            return entity;
        }

        public ToManyGuidDocument PopulateFromEntity(ToManyGuid entity)
        {
            Id = entity.Id;

            return this;
        }

        public static ToManyGuidDocument? FromEntity(ToManyGuid? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ToManyGuidDocument().PopulateFromEntity(entity);
        }
    }
}