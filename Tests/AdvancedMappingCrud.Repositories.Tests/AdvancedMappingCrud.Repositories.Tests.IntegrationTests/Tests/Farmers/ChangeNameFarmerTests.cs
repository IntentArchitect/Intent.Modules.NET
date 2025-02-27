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
    public class ChangeNameFarmerTests : BaseIntegrationTest
    {
        public ChangeNameFarmerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ChangeNameFarmer_ShouldChangeNameFarmer()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var farmerId = await dataFactory.CreateFarmer();

            var command = dataFactory.CreateCommand<ChangeNameFarmerCommand>();
            command.Id = farmerId;

            // Act
            await client.ChangeNameFarmerAsync(farmerId, command);

            // Assert
            var farmer = await client.GetFarmerByIdAsync(farmerId);
            Assert.NotNull(farmer);
            Assert.Equal(command.Name, farmer.Name);
        }
    }
}