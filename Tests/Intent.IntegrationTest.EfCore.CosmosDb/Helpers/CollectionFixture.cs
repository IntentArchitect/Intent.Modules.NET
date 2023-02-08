using Xunit;

namespace Intent.IntegrationTest.EfCore.CosmosDb.Helpers;

[CollectionDefinition(CollectionDefinitionName)]
public class CollectionFixture : ICollectionFixture<DataContainerFixture>
{
    public const string CollectionDefinitionName = "Tests Collection";
}