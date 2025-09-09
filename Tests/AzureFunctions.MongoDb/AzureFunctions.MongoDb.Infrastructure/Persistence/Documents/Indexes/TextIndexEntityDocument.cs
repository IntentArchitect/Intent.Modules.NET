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
    internal class TextIndexEntityDocument : ITextIndexEntityDocument, IMongoDbDocument<TextIndexEntity, TextIndexEntityDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string FullText { get; set; }
        public string SomeField { get; set; }

        public TextIndexEntity ToEntity(TextIndexEntity? entity = default)
        {
            entity ??= new TextIndexEntity();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.FullText = FullText ?? throw new Exception($"{nameof(entity.FullText)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");

            return entity;
        }

        public TextIndexEntityDocument PopulateFromEntity(TextIndexEntity entity)
        {
            Id = entity.Id;
            FullText = entity.FullText;
            SomeField = entity.SomeField;

            return this;
        }

        public static TextIndexEntityDocument? FromEntity(TextIndexEntity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TextIndexEntityDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<TextIndexEntityDocument> GetIdFilter(string id)
        {
            return Builders<TextIndexEntityDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<TextIndexEntityDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<TextIndexEntityDocument> GetIdsFilter(string[] ids)
        {
            return Builders<TextIndexEntityDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<TextIndexEntityDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<TextIndexEntityDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}