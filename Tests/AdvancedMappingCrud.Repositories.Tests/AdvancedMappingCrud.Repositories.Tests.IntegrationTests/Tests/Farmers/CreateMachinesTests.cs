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
    public class CreateMachinesTests : BaseIntegrationTest
    {
        public CreateMachinesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateMachines_ShouldCreateMachines()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var farmerId = await dataFactory.CreateMachinesDependencies();

            var command = dataFactory.CreateCommand<CreateMachinesCommand>();

            // Act
            var machinesId = await client.CreateMachinesAsync(farmerId, command, TestContext.Current.CancellationToken);

            // Assert
            var machines = await client.GetMachinesByIdAsync(farmerId, machinesId, TestContext.Current.CancellationToken);
            Assert.NotNull(machines);
        }
    }
}