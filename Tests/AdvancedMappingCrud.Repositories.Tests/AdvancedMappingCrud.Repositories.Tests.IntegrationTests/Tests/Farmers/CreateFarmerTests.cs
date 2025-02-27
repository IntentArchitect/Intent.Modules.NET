using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Farmers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateFarmerTests : BaseIntegrationTest
    {
        public CreateFarmerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateFarmer_ShouldCreateFarmer()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateFarmerCommand>();

            // Act
            var farmerId = await client.CreateFarmerAsync(command);

            // Assert
            var farmer = await client.GetFarmerByIdAsync(farmerId);
            Assert.NotNull(farmer);
        }
    }
}