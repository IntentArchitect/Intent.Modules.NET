using AzureFunctions.MongoDb.Domain.Entities.Collections;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Collections
{
    public class CustomCollectionEntityAMapping : IMongoMappingConfiguration<CustomCollectionEntityA>
    {
        public string CollectionName => "CustomCollection";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomCollectionEntityA)))
            {
                BsonClassMap.RegisterClassMap<CustomCollectionEntityA>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}