using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.CheckNewCompChildCruds;
using IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateCNCCChildTests : BaseIntegrationTest
    {
        public UpdateCNCCChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateCNCCChild_ShouldUpdateCNCCChild()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateCNCCChild();

            var command = dataFactory.CreateCommand<UpdateCNCCChildCommand>();
            command.Id = ids.CNCCChildId;

            // Act
            await client.UpdateCNCCChildAsync(ids.CheckNewCompChildCrudId, ids.CNCCChildId, command);

            // Assert
            var cNCCChild = await client.GetCNCCChildByIdAsync(ids.CheckNewCompChildCrudId, ids.CNCCChildId);
            Assert.NotNull(cNCCChild);
            Assert.Equal(command.Description, cNCCChild.Description);
        }
    }
}