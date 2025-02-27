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
    public class ChangeNameMachinesTests : BaseIntegrationTest
    {
        public ChangeNameMachinesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ChangeNameMachines_ShouldChangeNameMachines()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateMachines();

            var command = dataFactory.CreateCommand<ChangeNameMachinesCommand>();
            command.Id = ids.MachinesId;

            // Act
            await client.ChangeNameMachinesAsync(ids.FarmerId, ids.MachinesId, command);

            // Assert
            var machines = await client.GetMachinesByIdAsync(ids.FarmerId, ids.MachinesId);
            Assert.NotNull(machines);
            Assert.Equal(command.Name, machines.Name);
        }
    }
}