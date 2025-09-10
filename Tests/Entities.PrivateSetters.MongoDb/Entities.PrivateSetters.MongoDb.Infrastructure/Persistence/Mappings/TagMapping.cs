using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Mappings
{
    public class TagMapping : IMongoMappingConfiguration<Tag>
    {
        public string CollectionName => "Tags";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Tag)))
            {
                BsonClassMap.RegisterClassMap<Tag>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.SetDiscriminator(nameof(Tag));
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}