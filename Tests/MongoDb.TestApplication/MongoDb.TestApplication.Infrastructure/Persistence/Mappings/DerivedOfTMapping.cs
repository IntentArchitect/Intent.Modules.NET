using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Mappings
{
    public class DerivedOfTMapping : IMongoMappingConfiguration<DerivedOfT>
    {
        public string CollectionName => "DerivedOfTs";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(DerivedOfT)))
            {
                BsonClassMap.RegisterClassMap<DerivedOfT>(
                    mapping =>
                    {
                        mapping.AutoMap();
                    });
            }
        }
    }
}