using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetClientByIdTests : BaseIntegrationTest
    {
        public GetClientByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetClientById_ShouldGetClientById()
        {
            // Arrange
            var client = new ClientsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var clientId = await dataFactory.CreateClient();

            // Act
            var clientEntity = await client.GetClientByIdAsync(clientId);

            // Assert
            Assert.NotNull(clientEntity);
        }
    }
}