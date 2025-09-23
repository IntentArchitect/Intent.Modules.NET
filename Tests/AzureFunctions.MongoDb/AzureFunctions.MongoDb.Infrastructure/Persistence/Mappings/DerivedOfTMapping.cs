using AzureFunctions.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings
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