using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Children;
using IntegrationTesting.Tests.IntegrationTests.Services.Children;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateChildTests : BaseIntegrationTest
    {
        public CreateChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateChild_ShouldCreateChild()
        {
            // Arrange
            var client = new ChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateChildDependencies();

            var command = dataFactory.CreateCommand<CreateChildCommand>();

            // Act
            var childId = await client.CreateChildAsync(command);

            // Assert
            var child = await client.GetChildByIdAsync(childId);
            Assert.NotNull(child);
        }
    }
}