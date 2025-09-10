using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoMappingConfigurationInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Persistence.Mappings
{
    public interface IMongoMappingConfiguration<T>
    {
        string CollectionName { get; }
        void RegisterCollectionMap();
    }
}