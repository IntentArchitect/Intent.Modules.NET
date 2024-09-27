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
    public class CreateClientTests : BaseIntegrationTest
    {
        public CreateClientTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
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