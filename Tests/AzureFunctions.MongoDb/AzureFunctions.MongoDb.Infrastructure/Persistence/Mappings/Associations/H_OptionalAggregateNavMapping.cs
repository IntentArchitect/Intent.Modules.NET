using AzureFunctions.MongoDb.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.Associations
{
    public class H_OptionalAggregateNavMapping : IMongoMappingConfiguration<H_OptionalAggregateNav>
    {
        public string CollectionName => "H_OptionalAggregateNavs";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(H_OptionalAggregateNav)))
            {
                BsonClassMap.RegisterClassMap<H_OptionalAggregateNav>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}