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