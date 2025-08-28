using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Indexes;
using MongoDb.TestApplication.Domain.Repositories.Documents.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Indexes
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