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