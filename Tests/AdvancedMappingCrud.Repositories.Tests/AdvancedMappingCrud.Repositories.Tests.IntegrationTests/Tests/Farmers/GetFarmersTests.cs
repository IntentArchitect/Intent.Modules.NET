using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Farmers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetFarmersTests : BaseIntegrationTest
    {
        public GetFarmersTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetFarmers_ShouldGetFarmers()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateFarmer();

            // Act
            var farmers = await client.GetFarmersAsync();

            // Assert
            Assert.NotEmpty(farmers);
        }
    }
}