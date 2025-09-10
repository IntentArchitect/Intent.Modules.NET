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
    public class InvoiceMapping : IMongoMappingConfiguration<Invoice>
    {
        public string CollectionName => "Invoices";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Invoice)))
            {
                BsonClassMap.RegisterClassMap<Invoice>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.SetDiscriminator(nameof(Invoice));
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}