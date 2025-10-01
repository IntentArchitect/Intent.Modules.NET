using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb.TestApplication.Domain.Entities.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Mappings.Collections
{
    public class CustomCollectionEntityBMapping : IMongoMappingConfiguration<CustomCollectionEntityB>
    {
        public string CollectionName => "CustomCollection";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomCollectionEntityB)))
            {
                BsonClassMap.RegisterClassMap<CustomCollectionEntityB>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}