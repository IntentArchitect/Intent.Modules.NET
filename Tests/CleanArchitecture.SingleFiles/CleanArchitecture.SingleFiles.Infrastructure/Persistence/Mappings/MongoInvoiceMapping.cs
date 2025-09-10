using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Mappings
{
    public class MongoInvoiceMapping : IMongoMappingConfiguration<MongoInvoice>
    {
        public string CollectionName => "MongoInvoices";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(MongoInvoice)))
            {
                BsonClassMap.RegisterClassMap<MongoInvoice>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.SetDiscriminator(nameof(MongoInvoice));
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance).SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                    });
            }
        }
    }
}