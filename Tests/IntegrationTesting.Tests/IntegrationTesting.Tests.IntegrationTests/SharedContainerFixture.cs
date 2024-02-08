using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.SharedContainerFixture", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests
{
    [CollectionDefinition("SharedContainer")]
    public class SharedContainerFixture : ICollectionFixture<IntegrationTestWebAppFactory>
    {
    }
}