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
    internal class CompoundIndexEntitySingleChildDocument : ICompoundIndexEntitySingleChildDocument
    {
        public string CompoundOne { get; set; } = default!;
        public string CompoundTwo { get; set; } = default!;

        public CompoundIndexEntitySingleChild ToEntity(CompoundIndexEntitySingleChild? entity = default)
        {
            entity ??= new CompoundIndexEntitySingleChild();

            entity.CompoundOne = CompoundOne ?? throw new Exception($"{nameof(entity.CompoundOne)} is null");
            entity.CompoundTwo = CompoundTwo ?? throw new Exception($"{nameof(entity.CompoundTwo)} is null");

            return entity;
        }

        public CompoundIndexEntitySingleChildDocument PopulateFromEntity(CompoundIndexEntitySingleChild entity)
        {
            CompoundOne = entity.CompoundOne;
            CompoundTwo = entity.CompoundTwo;

            return this;
        }

        public static CompoundIndexEntitySingleChildDocument? FromEntity(CompoundIndexEntitySingleChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CompoundIndexEntitySingleChildDocument().PopulateFromEntity(entity);
        }
    }
}