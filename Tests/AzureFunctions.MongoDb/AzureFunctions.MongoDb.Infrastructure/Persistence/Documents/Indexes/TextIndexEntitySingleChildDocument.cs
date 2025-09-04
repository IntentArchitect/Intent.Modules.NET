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
    [BsonDiscriminator(nameof(TextIndexEntitySingleChild), Required = true)]
    internal class TextIndexEntitySingleChildDocument : ITextIndexEntitySingleChildDocument
    {
        public string FullText { get; set; } = default!;

        public TextIndexEntitySingleChild ToEntity(TextIndexEntitySingleChild? entity = default)
        {
            entity ??= new TextIndexEntitySingleChild();

            entity.FullText = FullText ?? throw new Exception($"{nameof(entity.FullText)} is null");

            return entity;
        }

        public TextIndexEntitySingleChildDocument PopulateFromEntity(TextIndexEntitySingleChild entity)
        {
            FullText = entity.FullText;

            return this;
        }

        public static TextIndexEntitySingleChildDocument? FromEntity(TextIndexEntitySingleChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TextIndexEntitySingleChildDocument().PopulateFromEntity(entity);
        }
    }
}