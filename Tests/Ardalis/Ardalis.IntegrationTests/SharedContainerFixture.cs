using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.SharedContainerFixture", Version = "1.0")]

namespace Ardalis.IntegrationTests
{
    [CollectionDefinition("SharedContainer")]
    public class SharedContainerFixture : ICollectionFixture<IntegrationTestWebAppFactory>
    {
    }
}