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
    public class CreatePartialCrudTests : BaseIntegrationTest
    {
        public CreatePartialCrudTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreatePartialCrud_ShouldCreatePartialCrud()
        {
            // Arrange
            var client = new PartialCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreatePartialCrudCommand>();

            // Act
            var partialCrudId = await client.CreatePartialCrudAsync(command);

            // Assert
            var partialCrud = await client.GetPartialCrudByIdAsync(partialCrudId);
            Assert.NotNull(partialCrud);
        }
    }
}