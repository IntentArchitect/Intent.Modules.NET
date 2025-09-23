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
    public class DerivedMapping : IMongoMappingConfiguration<Derived>
    {
        public string CollectionName => "Deriveds";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Derived)))
            {
                BsonClassMap.RegisterClassMap<Derived>(
                    mapping =>
                    {
                        mapping.AutoMap();
                    });
            }
        }
    }
}