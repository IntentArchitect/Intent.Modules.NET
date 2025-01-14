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
    public class CreateCheckNewCompChildCrudTests : BaseIntegrationTest
    {
        public CreateCheckNewCompChildCrudTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateCheckNewCompChildCrud_ShouldCreateCheckNewCompChildCrud()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateCheckNewCompChildCrudCommand>();

            // Act
            var checkNewCompChildCrudId = await client.CreateCheckNewCompChildCrudAsync(command);

            // Assert
            var checkNewCompChildCrud = await client.GetCheckNewCompChildCrudByIdAsync(checkNewCompChildCrudId);
            Assert.NotNull(checkNewCompChildCrud);
        }
    }
}