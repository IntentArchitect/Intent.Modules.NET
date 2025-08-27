using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.ToManyIds
{
    internal class ToManyLongDocument : IToManyLongDocument
    {
        public long Id { get; set; }

        public ToManyLong ToEntity(ToManyLong? entity = default)
        {
            entity ??= new ToManyLong();

            entity.Id = Id;

            return entity;
        }

        public ToManyLongDocument PopulateFromEntity(ToManyLong entity)
        {
            Id = entity.Id;

            return this;
        }

        public static ToManyLongDocument? FromEntity(ToManyLong? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ToManyLongDocument().PopulateFromEntity(entity);
        }
    }
}