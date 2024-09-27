using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.PartialCruds;
using IntegrationTesting.Tests.IntegrationTests.Services.PartialCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdatePartialCrudTests : BaseIntegrationTest
    {
        public UpdatePartialCrudTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdatePartialCrud_ShouldUpdatePartialCrud()
        {
            // Arrange
            var client = new PartialCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var partialCrudId = await dataFactory.CreatePartialCrud();

            var command = dataFactory.CreateCommand<UpdatePartialCrudCommand>();
            command.Id = partialCrudId;

            // Act
            await client.UpdatePartialCrudAsync(partialCrudId, command);

            // Assert
            var partialCrud = await client.GetPartialCrudByIdAsync(partialCrudId);
            Assert.NotNull(partialCrud);
            Assert.Equal(command.Name, partialCrud.Name);
        }
    }
}