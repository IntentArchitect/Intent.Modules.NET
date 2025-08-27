using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes
{
    internal class SingleIndexEntityMultiChildDocument : ISingleIndexEntityMultiChildDocument
    {
        public string Id { get; set; } = default!;
        public string SingleIndex { get; set; } = default!;

        public SingleIndexEntityMultiChild ToEntity(SingleIndexEntityMultiChild? entity = default)
        {
            entity ??= new SingleIndexEntityMultiChild();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SingleIndex = SingleIndex ?? throw new Exception($"{nameof(entity.SingleIndex)} is null");

            return entity;
        }

        public SingleIndexEntityMultiChildDocument PopulateFromEntity(SingleIndexEntityMultiChild entity)
        {
            Id = entity.Id;
            SingleIndex = entity.SingleIndex;

            return this;
        }

        public static SingleIndexEntityMultiChildDocument? FromEntity(SingleIndexEntityMultiChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SingleIndexEntityMultiChildDocument().PopulateFromEntity(entity);
        }
    }
}