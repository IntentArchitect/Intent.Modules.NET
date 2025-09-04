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
    [BsonDiscriminator(nameof(MultikeyIndexEntityMultiChild), Required = true)]
    internal class MultikeyIndexEntityMultiChildDocument : IMultikeyIndexEntityMultiChildDocument
    {
        public string Id { get; set; } = default!;
        public IEnumerable<string> MultiKey { get; set; } = default!;

        public MultikeyIndexEntityMultiChild ToEntity(MultikeyIndexEntityMultiChild? entity = default)
        {
            entity ??= new MultikeyIndexEntityMultiChild();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.MultiKey = MultiKey.ToList() ?? throw new Exception($"{nameof(entity.MultiKey)} is null");

            return entity;
        }

        public MultikeyIndexEntityMultiChildDocument PopulateFromEntity(MultikeyIndexEntityMultiChild entity)
        {
            Id = entity.Id;
            MultiKey = entity.MultiKey.ToList();

            return this;
        }

        public static MultikeyIndexEntityMultiChildDocument? FromEntity(MultikeyIndexEntityMultiChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MultikeyIndexEntityMultiChildDocument().PopulateFromEntity(entity);
        }
    }
}