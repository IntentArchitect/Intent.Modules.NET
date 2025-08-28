using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Collections.FolderCollection;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Collections.FolderCollection;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Collections.FolderCollection
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

        public static Expression<Func<FolderCollectionEntityBDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<FolderCollectionEntityBDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}