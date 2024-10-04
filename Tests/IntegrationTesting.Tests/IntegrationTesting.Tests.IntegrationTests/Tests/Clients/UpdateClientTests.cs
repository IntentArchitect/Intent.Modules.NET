using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Clients;
using IntegrationTesting.Tests.IntegrationTests.Services.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateClientTests : BaseIntegrationTest
    {
        public UpdateClientTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateClient_ShouldUpdateClient()
        {
            // Arrange
            var client = new ClientsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var clientId = await dataFactory.CreateClient();

            var command = dataFactory.CreateCommand<UpdateClientCommand>();
            command.Id = clientId;

            // Act
            await client.UpdateClientAsync(clientId, command);

            // Assert
            var clientEntity = await client.GetClientByIdAsync(clientId);
            Assert.NotNull(clientEntity);
            Assert.Equal(command.Name, clientEntity.Name);
        }
    }
}