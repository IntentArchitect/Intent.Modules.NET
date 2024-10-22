using Ardalis.IntegrationTests.HttpClients.Clients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace Ardalis.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetClientsTests : BaseIntegrationTest
    {
        public GetClientsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        //[Fact]
        [IntentIgnore]
        public async Task GetClients_ShouldGetClients()
        {
            // Arrange
            var client = new ClientsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateClient();

            // Act
            var clients = await client.GetClientsAsync();

            // Assert
            Assert.True(clients.Count > 0);
        }
    }
}