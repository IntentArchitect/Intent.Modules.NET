using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Collections.FolderCollection;
using MongoDb.TestApplication.Domain.Repositories.Documents.Collections.FolderCollection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections.FolderCollection
{
    internal class FolderCollectionEntityBDocument : IFolderCollectionEntityBDocument, IMongoDbDocument<FolderCollectionEntityB, FolderCollectionEntityBDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public FolderCollectionEntityB ToEntity(FolderCollectionEntityB? entity = default)
        {
            entity ??= new FolderCollectionEntityB();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public FolderCollectionEntityBDocument PopulateFromEntity(FolderCollectionEntityB entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static FolderCollectionEntityBDocument? FromEntity(FolderCollectionEntityB? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FolderCollectionEntityBDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<FolderCollectionEntityBDocument> GetIdFilter(string id)
        {
            return Builders<FolderCollectionEntityBDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<FolderCollectionEntityBDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<FolderCollectionEntityBDocument> GetIdsFilter(string[] ids)
        {
            return Builders<FolderCollectionEntityBDocument>.Filter.In(d => d.Id, ids);
        }
    }
}