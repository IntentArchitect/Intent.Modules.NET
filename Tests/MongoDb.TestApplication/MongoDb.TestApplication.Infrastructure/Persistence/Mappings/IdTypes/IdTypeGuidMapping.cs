using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbMapping", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Mappings.IdTypes
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