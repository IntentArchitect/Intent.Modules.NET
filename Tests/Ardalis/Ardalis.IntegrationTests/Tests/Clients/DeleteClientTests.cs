using System.Net;
using Ardalis.IntegrationTests.HttpClients;
using Ardalis.IntegrationTests.HttpClients.Clients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace Ardalis.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteClientTests : BaseIntegrationTest
    {
        public DeleteClientTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        //[Fact]
        [IntentIgnore]
        public async Task DeleteClient_ShouldDeleteClient()
        {
            // Arrange
            var client = new ClientsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var clientId = await dataFactory.CreateClient();

            // Act
            await client.DeleteClientAsync(clientId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetClientByIdAsync(clientId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}