using System;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes
{
    internal class SingleIndexEntitySingleChildDocument : ISingleIndexEntitySingleChildDocument
    {
        public string SingleIndex { get; set; } = default!;

        public SingleIndexEntitySingleChild ToEntity(SingleIndexEntitySingleChild? entity = default)
        {
            entity ??= new SingleIndexEntitySingleChild();

            entity.SingleIndex = SingleIndex ?? throw new Exception($"{nameof(entity.SingleIndex)} is null");

            return entity;
        }

        public SingleIndexEntitySingleChildDocument PopulateFromEntity(SingleIndexEntitySingleChild entity)
        {
            SingleIndex = entity.SingleIndex;

            return this;
        }

        public static SingleIndexEntitySingleChildDocument? FromEntity(SingleIndexEntitySingleChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SingleIndexEntitySingleChildDocument().PopulateFromEntity(entity);
        }
    }
}