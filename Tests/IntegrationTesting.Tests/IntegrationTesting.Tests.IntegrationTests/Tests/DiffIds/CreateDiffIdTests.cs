using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DiffIds;
using IntegrationTesting.Tests.IntegrationTests.Services.DiffIds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateDiffIdTests : BaseIntegrationTest
    {
        public CreateDiffIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateDiffId_ShouldCreateDiffId()
        {
            // Arrange
            var integrationClient = new DiffIdsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateDiffIdCommand>();

            // Act
            var diffIdId = await integrationClient.CreateDiffIdAsync(command);

            // Assert
            var diffId = await integrationClient.GetDiffIdByIdAsync(diffIdId);
            Assert.NotNull(diffId);
        }
    }
}