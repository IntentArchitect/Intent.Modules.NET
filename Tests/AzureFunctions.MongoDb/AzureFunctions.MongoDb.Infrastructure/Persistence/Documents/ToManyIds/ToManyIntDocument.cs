using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.ToManyIds
{
    internal class ToManyIntDocument : IToManyIntDocument
    {
        public int Id { get; set; }

        public ToManyInt ToEntity(ToManyInt? entity = default)
        {
            entity ??= new ToManyInt();

            entity.Id = Id;

            return entity;
        }

        public ToManyIntDocument PopulateFromEntity(ToManyInt entity)
        {
            Id = entity.Id;

            return this;
        }

        public static ToManyIntDocument? FromEntity(ToManyInt? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ToManyIntDocument().PopulateFromEntity(entity);
        }
    }
}