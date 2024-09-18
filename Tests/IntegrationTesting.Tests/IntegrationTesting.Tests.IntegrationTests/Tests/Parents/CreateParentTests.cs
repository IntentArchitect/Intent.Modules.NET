using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Parents;
using IntegrationTesting.Tests.IntegrationTests.Services.Parents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateParentTests : BaseIntegrationTest
    {
        public CreateParentTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateParent_ShouldCreateParent()
        {
            // Arrange
            var integrationClient = new ParentsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateParentCommand>();

            // Act
            var parentId = await integrationClient.CreateParentAsync(command);

            // Assert
            var parent = await integrationClient.GetParentByIdAsync(parentId);
            Assert.NotNull(parent);
        }
    }
}