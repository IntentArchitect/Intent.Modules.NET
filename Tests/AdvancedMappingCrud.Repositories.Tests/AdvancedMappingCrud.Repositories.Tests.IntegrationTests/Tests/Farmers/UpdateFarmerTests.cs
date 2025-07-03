using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Farmers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.Farmers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateFarmerTests : BaseIntegrationTest
    {
        public UpdateFarmerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateFarmer_ShouldUpdateFarmer()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var farmerId = await dataFactory.CreateFarmer();

            var command = dataFactory.CreateCommand<UpdateFarmerCommand>();
            command.Id = farmerId;

            // Act
            await client.UpdateFarmerAsync(farmerId, command, TestContext.Current.CancellationToken);

            // Assert
            var farmer = await client.GetFarmerByIdAsync(farmerId, TestContext.Current.CancellationToken);
            Assert.NotNull(farmer);
            Assert.Equal(command.Name, farmer.Name);
        }
    }
}