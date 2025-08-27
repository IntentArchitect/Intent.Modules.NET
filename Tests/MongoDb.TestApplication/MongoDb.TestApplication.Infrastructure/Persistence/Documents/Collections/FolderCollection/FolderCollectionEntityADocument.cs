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
    internal class FolderCollectionEntityADocument : IFolderCollectionEntityADocument, IMongoDbDocument<FolderCollectionEntityA, FolderCollectionEntityADocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }

        public FolderCollectionEntityA ToEntity(FolderCollectionEntityA? entity = default)
        {
            entity ??= new FolderCollectionEntityA();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public FolderCollectionEntityADocument PopulateFromEntity(FolderCollectionEntityA entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static FolderCollectionEntityADocument? FromEntity(FolderCollectionEntityA? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FolderCollectionEntityADocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<FolderCollectionEntityADocument> GetIdFilter(string id)
        {
            return Builders<FolderCollectionEntityADocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<FolderCollectionEntityADocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<FolderCollectionEntityADocument> GetIdsFilter(string[] ids)
        {
            return Builders<FolderCollectionEntityADocument>.Filter.In(d => d.Id, ids);
        }
    }
}