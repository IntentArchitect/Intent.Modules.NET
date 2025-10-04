using AzureFunctions.MongoDb.Domain.Entities.Collections.FolderCollection;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Collections.FolderCollection
{
    public class FolderCollectionEntityBMapping : IMongoMappingConfiguration<FolderCollectionEntityB>
    {
        public string CollectionName => "FolderCollection";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(FolderCollectionEntityB)))
            {
                BsonClassMap.RegisterClassMap<FolderCollectionEntityB>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}