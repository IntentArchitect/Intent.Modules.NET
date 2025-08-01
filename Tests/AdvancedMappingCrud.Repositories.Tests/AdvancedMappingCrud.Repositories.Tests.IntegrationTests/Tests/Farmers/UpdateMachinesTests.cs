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
    public class UpdateMachinesTests : BaseIntegrationTest
    {
        public UpdateMachinesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateMachines_ShouldUpdateMachines()
        {
            // Arrange
            var client = new FarmersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateMachines();

            var command = dataFactory.CreateCommand<UpdateMachinesCommand>();
            command.Id = ids.MachinesId;

            // Act
            await client.UpdateMachinesAsync(ids.FarmerId, ids.MachinesId, command, TestContext.Current.CancellationToken);

            // Assert
            var machines = await client.GetMachinesByIdAsync(ids.FarmerId, ids.MachinesId, TestContext.Current.CancellationToken);
            Assert.NotNull(machines);
            Assert.Equal(command.Name, machines.Name);
        }
    }
}