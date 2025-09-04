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
    [BsonDiscriminator(nameof(CompoundIndexEntityMultiChild), Required = true)]
    internal class CompoundIndexEntityMultiChildDocument : ICompoundIndexEntityMultiChildDocument
    {
        public string Id { get; set; } = default!;
        public string CompoundOne { get; set; } = default!;
        public string CompoundTwo { get; set; } = default!;

        public CompoundIndexEntityMultiChild ToEntity(CompoundIndexEntityMultiChild? entity = default)
        {
            entity ??= new CompoundIndexEntityMultiChild();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CompoundOne = CompoundOne ?? throw new Exception($"{nameof(entity.CompoundOne)} is null");
            entity.CompoundTwo = CompoundTwo ?? throw new Exception($"{nameof(entity.CompoundTwo)} is null");

            return entity;
        }

        public CompoundIndexEntityMultiChildDocument PopulateFromEntity(CompoundIndexEntityMultiChild entity)
        {
            Id = entity.Id;
            CompoundOne = entity.CompoundOne;
            CompoundTwo = entity.CompoundTwo;

            return this;
        }

        public static CompoundIndexEntityMultiChildDocument? FromEntity(CompoundIndexEntityMultiChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CompoundIndexEntityMultiChildDocument().PopulateFromEntity(entity);
        }
    }
}