using Ardalis.IntegrationTests.HttpClients.Clients;
using Ardalis.IntegrationTests.Services.Clients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace Ardalis.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateClientTests : BaseIntegrationTest
    {
        public UpdateClientTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        //[Fact]
        [IntentIgnore]
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