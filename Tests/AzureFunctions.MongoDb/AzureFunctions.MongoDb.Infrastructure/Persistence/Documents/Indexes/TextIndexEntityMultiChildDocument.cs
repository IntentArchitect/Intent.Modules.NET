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
    [BsonDiscriminator(nameof(TextIndexEntityMultiChild), Required = true)]
    internal class TextIndexEntityMultiChildDocument : ITextIndexEntityMultiChildDocument
    {
        public string Id { get; set; } = default!;
        public string FullText { get; set; } = default!;

        public TextIndexEntityMultiChild ToEntity(TextIndexEntityMultiChild? entity = default)
        {
            entity ??= new TextIndexEntityMultiChild();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.FullText = FullText ?? throw new Exception($"{nameof(entity.FullText)} is null");

            return entity;
        }

        public TextIndexEntityMultiChildDocument PopulateFromEntity(TextIndexEntityMultiChild entity)
        {
            Id = entity.Id;
            FullText = entity.FullText;

            return this;
        }

        public static TextIndexEntityMultiChildDocument? FromEntity(TextIndexEntityMultiChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TextIndexEntityMultiChildDocument().PopulateFromEntity(entity);
        }
    }
}