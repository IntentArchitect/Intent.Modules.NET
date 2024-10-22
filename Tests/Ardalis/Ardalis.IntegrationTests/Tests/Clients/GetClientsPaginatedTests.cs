using Ardalis.IntegrationTests.HttpClients.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace Ardalis.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetClientsPaginatedTests : BaseIntegrationTest
    {
        public GetClientsPaginatedTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetClientsPaginated_ShouldGetClients()
        {
            // Arrange
            var client = new ClientsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateClient();

            // Act
            var clients = await client.GetClientsPaginatedAsync(1, 2);

            // Assert
            Assert.True(clients.Data.Count > 0);
        }

    }
}