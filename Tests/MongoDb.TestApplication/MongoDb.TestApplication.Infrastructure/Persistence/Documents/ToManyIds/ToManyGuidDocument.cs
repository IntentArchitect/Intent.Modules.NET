using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.ToManyIds;
using MongoDb.TestApplication.Domain.Repositories.Documents.ToManyIds;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.ToManyIds
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