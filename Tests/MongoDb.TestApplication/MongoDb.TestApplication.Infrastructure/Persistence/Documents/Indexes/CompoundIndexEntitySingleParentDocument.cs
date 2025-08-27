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
    internal class CompoundIndexEntitySingleParentDocument : ICompoundIndexEntitySingleParentDocument, IMongoDbDocument<CompoundIndexEntitySingleParent, CompoundIndexEntitySingleParentDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string SomeField { get; set; }
        public ICompoundIndexEntitySingleChildDocument CompoundIndexEntitySingleChild { get; set; }

        public CompoundIndexEntitySingleParent ToEntity(CompoundIndexEntitySingleParent? entity = default)
        {
            entity ??= new CompoundIndexEntitySingleParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SomeField = SomeField ?? throw new Exception($"{nameof(entity.SomeField)} is null");
            entity.CompoundIndexEntitySingleChild = (CompoundIndexEntitySingleChild as CompoundIndexEntitySingleChildDocument).ToEntity() ?? throw new Exception($"{nameof(entity.CompoundIndexEntitySingleChild)} is null");

            return entity;
        }

        public CompoundIndexEntitySingleParentDocument PopulateFromEntity(CompoundIndexEntitySingleParent entity)
        {
            Id = entity.Id;
            SomeField = entity.SomeField;
            CompoundIndexEntitySingleChild = CompoundIndexEntitySingleChildDocument.FromEntity(entity.CompoundIndexEntitySingleChild)!;

            return this;
        }

        public static CompoundIndexEntitySingleParentDocument? FromEntity(CompoundIndexEntitySingleParent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CompoundIndexEntitySingleParentDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<CompoundIndexEntitySingleParentDocument> GetIdFilter(string id)
        {
            return Builders<CompoundIndexEntitySingleParentDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<CompoundIndexEntitySingleParentDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<CompoundIndexEntitySingleParentDocument> GetIdsFilter(string[] ids)
        {
            return Builders<CompoundIndexEntitySingleParentDocument>.Filter.In(d => d.Id, ids);
        }
    }
}