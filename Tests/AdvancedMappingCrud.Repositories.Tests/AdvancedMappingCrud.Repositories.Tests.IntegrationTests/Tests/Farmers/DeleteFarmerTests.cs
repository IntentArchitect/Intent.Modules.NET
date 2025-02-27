using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Farmers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteFarmerTests : BaseIntegrationTest
    {
        public DeleteFarmerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteFarmer_ShouldDeleteFarmer()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var farmerId = await dataFactory.CreateFarmer();

            // Act
            await client.DeleteFarmerAsync(farmerId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetFarmerByIdAsync(farmerId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}