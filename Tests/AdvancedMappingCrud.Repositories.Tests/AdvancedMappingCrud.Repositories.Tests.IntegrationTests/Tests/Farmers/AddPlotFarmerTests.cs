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
    public class AddPlotFarmerTests : BaseIntegrationTest
    {
        public AddPlotFarmerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AddPlotFarmer_ShouldAddPlotFarmer()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var farmerId = await dataFactory.CreateFarmer();

            var command = dataFactory.CreateCommand<AddPlotFarmerCommand>();
            command.Id = farmerId;

            // Act
            await client.AddPlotFarmerAsync(farmerId, command);

            // Assert
            var farmer = await client.GetFarmerByIdAsync(farmerId);
            Assert.NotNull(farmer);
        }
    }
}