using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Infrastructure.Persistence.Mappings.CollaborativeEditing
{
    public class DocumentMapping : IMongoMappingConfiguration<Document>
    {
        public string CollectionName => "Documents";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Document)))
            {
                BsonClassMap.RegisterClassMap<Document>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.SetDiscriminator(nameof(Document));
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                    });
            }
        }
    }
}