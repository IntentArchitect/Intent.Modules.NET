using System;
using System.Collections.Generic;
using System.Linq;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes
{
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