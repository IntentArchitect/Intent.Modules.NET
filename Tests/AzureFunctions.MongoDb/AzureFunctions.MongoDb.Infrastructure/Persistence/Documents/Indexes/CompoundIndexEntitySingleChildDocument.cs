using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes
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