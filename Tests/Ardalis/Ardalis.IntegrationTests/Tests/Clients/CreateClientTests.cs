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
    public class CreateClientTests : BaseIntegrationTest
    {
        public CreateClientTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        //[Fact]
        [IntentIgnore]
        public async Task CreateClient_ShouldCreateClient()
        {
            // Arrange
            var client = new ClientsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateClientCommand>();

            // Act
            var clientId = await client.CreateClientAsync(command);

            // Assert
            var clientEntity = await client.GetClientByIdAsync(clientId);
            Assert.NotNull(clientEntity);
        }
    }
}