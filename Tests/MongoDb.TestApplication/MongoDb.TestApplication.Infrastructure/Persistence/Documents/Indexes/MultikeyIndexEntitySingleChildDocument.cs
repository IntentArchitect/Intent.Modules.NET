using System;
using System.Collections.Generic;
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
    [BsonDiscriminator(nameof(MultikeyIndexEntitySingleChild), Required = true)]
    internal class MultikeyIndexEntitySingleChildDocument : IMultikeyIndexEntitySingleChildDocument
    {
        public IEnumerable<string> MultiKey { get; set; } = default!;

        public MultikeyIndexEntitySingleChild ToEntity(MultikeyIndexEntitySingleChild? entity = default)
        {
            entity ??= new MultikeyIndexEntitySingleChild();

            entity.MultiKey = MultiKey.ToList() ?? throw new Exception($"{nameof(entity.MultiKey)} is null");

            return entity;
        }

        public MultikeyIndexEntitySingleChildDocument PopulateFromEntity(MultikeyIndexEntitySingleChild entity)
        {
            MultiKey = entity.MultiKey.ToList();

            return this;
        }

        public static MultikeyIndexEntitySingleChildDocument? FromEntity(MultikeyIndexEntitySingleChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MultikeyIndexEntitySingleChildDocument().PopulateFromEntity(entity);
        }
    }
}