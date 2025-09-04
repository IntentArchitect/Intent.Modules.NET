using System;
using System.Collections.Generic;
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
    internal class TextIndexEntityMultiParentDocument : ITextIndexEntityMultiParentDocument, IMongoDbDocument<TextIndexEntityMultiParent, TextIndexEntityMultiParentDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public IEnumerable<ITextIndexEntityMultiChildDocument> TextIndexEntityMultiChild { get; set; }

        public TextIndexEntityMultiParent ToEntity(TextIndexEntityMultiParent? entity = default)
        {
            entity ??= new TextIndexEntityMultiParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.TextIndexEntityMultiChild = TextIndexEntityMultiChild.Select(x => (x as TextIndexEntityMultiChildDocument).ToEntity()).ToList();

            return entity;
        }

        public TextIndexEntityMultiParentDocument PopulateFromEntity(TextIndexEntityMultiParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            TextIndexEntityMultiChild = entity.TextIndexEntityMultiChild.Select(x => TextIndexEntityMultiChildDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static TextIndexEntityMultiParentDocument? FromEntity(TextIndexEntityMultiParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TextIndexEntityMultiParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<TextIndexEntityMultiParentDocument> GetIdFilter(string id)
        {
            return Builders<TextIndexEntityMultiParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<TextIndexEntityMultiParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<TextIndexEntityMultiParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<TextIndexEntityMultiParentDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<TextIndexEntityMultiParentDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<TextIndexEntityMultiParentDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}