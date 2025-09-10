using System;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Mappings.IdTypes
{
    public class IdTypeGuidMapping : IMongoMappingConfiguration<IdTypeGuid>
    {
        public string CollectionName => "IdTypeGuids";

        public void RegisterCollectionMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(IdTypeGuid)))
            {
                BsonClassMap.RegisterClassMap<IdTypeGuid>(
                    mapping =>
                    {
                        mapping.AutoMap();
                        mapping.MapIdMember(x => x.Id).SetIdGenerator(CombGuidGenerator.Instance).SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    });
            }
        }
    }
}