using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Mappings.Indexes
{
    public class MultikeyIndexEntitySingleParentMapping : IMongoMappingConfiguration<MultikeyIndexEntitySingleParent>
    {
        public string CollectionName => "MultikeyIndexEntitySingleParents";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(MultikeyIndexEntitySingleParent)))
            {
                BsonClassMap.RegisterClassMap<MultikeyIndexEntitySingleParent>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}