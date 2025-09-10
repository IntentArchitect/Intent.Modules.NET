using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Mappings
{
    public class MapImplyOptionalMapping : IMongoMappingConfiguration<MapImplyOptional>
    {
        public string CollectionName => "MapImplyOptionals";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(MapImplyOptional)))
            {
                BsonClassMap.RegisterClassMap<MapImplyOptional>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}