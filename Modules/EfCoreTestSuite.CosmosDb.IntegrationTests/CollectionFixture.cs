using Xunit;

namespace EfCoreTestSuite.CosmosDb.IntegrationTests;

[CollectionDefinition(CollectionDefinitionName)]
public class CollectionFixture : ICollectionFixture<DataContainerFixture>
{
    public const string CollectionDefinitionName = "Tests Collection";
}