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
    public class UpdateCheckNewCompChildCrudTests : BaseIntegrationTest
    {
        public UpdateCheckNewCompChildCrudTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateCheckNewCompChildCrud_ShouldUpdateCheckNewCompChildCrud()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var checkNewCompChildCrudId = await dataFactory.CreateCheckNewCompChildCrud();

            var command = dataFactory.CreateCommand<UpdateCheckNewCompChildCrudCommand>();
            command.Id = checkNewCompChildCrudId;

            // Act
            await client.UpdateCheckNewCompChildCrudAsync(checkNewCompChildCrudId, command);

            // Assert
            var checkNewCompChildCrud = await client.GetCheckNewCompChildCrudByIdAsync(checkNewCompChildCrudId);
            Assert.NotNull(checkNewCompChildCrud);
            Assert.Equal(command.Name, checkNewCompChildCrud.Name);
        }
    }
}