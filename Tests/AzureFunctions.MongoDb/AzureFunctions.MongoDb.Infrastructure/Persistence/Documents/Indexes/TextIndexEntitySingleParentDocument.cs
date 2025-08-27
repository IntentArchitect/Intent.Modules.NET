using System;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes
{
    internal class TextIndexEntitySingleParentDocument : ITextIndexEntitySingleParentDocument, IMongoDbDocument<TextIndexEntitySingleParent, TextIndexEntitySingleParentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public ITextIndexEntitySingleChildDocument TextIndexEntitySingleChild { get; set; }

        public TextIndexEntitySingleParent ToEntity(TextIndexEntitySingleParent? entity = default)
        {
            entity ??= new TextIndexEntitySingleParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.TextIndexEntitySingleChild = (TextIndexEntitySingleChild as TextIndexEntitySingleChildDocument).ToEntity() ?? throw new Exception($"{nameof(entity.TextIndexEntitySingleChild)} is null");

            return entity;
        }

        public TextIndexEntitySingleParentDocument PopulateFromEntity(TextIndexEntitySingleParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            TextIndexEntitySingleChild = TextIndexEntitySingleChildDocument.FromEntity(entity.TextIndexEntitySingleChild)!;

            return this;
        }

        public static TextIndexEntitySingleParentDocument? FromEntity(TextIndexEntitySingleParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TextIndexEntitySingleParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<TextIndexEntitySingleParentDocument> GetIdFilter(string id)
        {
            return Builders<TextIndexEntitySingleParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<TextIndexEntitySingleParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<TextIndexEntitySingleParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<TextIndexEntitySingleParentDocument>.Filter.In(d => d.Id, ids);
        }
    }
}