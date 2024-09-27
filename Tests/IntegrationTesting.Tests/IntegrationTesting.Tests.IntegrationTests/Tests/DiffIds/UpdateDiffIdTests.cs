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
    public class UpdateDiffIdTests : BaseIntegrationTest
    {
        public UpdateDiffIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateDiffId_ShouldUpdateDiffId()
        {
            // Arrange
            var client = new DiffIdsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var diffIdId = await dataFactory.CreateDiffId();

            var command = dataFactory.CreateCommand<UpdateDiffIdCommand>();
            command.MyId = diffIdId;

            // Act
            await client.UpdateDiffIdAsync(diffIdId, command);

            // Assert
            var diffId = await client.GetDiffIdByIdAsync(diffIdId);
            Assert.NotNull(diffId);
            Assert.Equal(command.Name, diffId.Name);
        }
    }
}