using Ardalis.IntegrationTests.HttpClients.Clients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace Ardalis.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetClientByIdTests : BaseIntegrationTest
    {
        public GetClientByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        //[Fact]
        [IntentIgnore]
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