using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Mappings
{
    public class ExternalDocMapping : IMongoMappingConfiguration<ExternalDoc>
    {
        public string CollectionName => "ExternalDocs";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(ExternalDoc)))
            {
                BsonClassMap.RegisterClassMap<ExternalDoc>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.SetDiscriminator(nameof(ExternalDoc));
                        mapping.MapIdMember(x => x.Id);
                    });
            }
        }
    }
}