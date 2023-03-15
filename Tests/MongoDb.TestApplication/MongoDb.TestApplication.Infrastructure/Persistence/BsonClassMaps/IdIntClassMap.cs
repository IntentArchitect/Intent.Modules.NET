using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Generators;
using MongoDB.Infrastructure;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.BsonClassMap", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.BsonClassMaps
{
    public class IdIntClassMap : IMongoDbFluentConfiguration
    {
        public void Configure()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(IdInt)))
            {
                return;
            }
            BsonClassMap.RegisterClassMap<IdInt>(
                build =>
                {
                    build.AutoMap();

                    build.MapIdProperty(c => c.Id)
                        .SetIdGenerator(Int32IdGenerator<IdInt>.Instance);
                });
        }
    }
}