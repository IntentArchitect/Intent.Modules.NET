using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.IntegrationTests.HttpClients.TestEntities;
using ValueObjects.Record.IntegrationTests.Services.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace ValueObjects.Record.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateTestEntityTests : BaseIntegrationTest
    {
        public UpdateTestEntityTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateTestEntity_ShouldUpdateTestEntity()
        {
            // Arrange
            var integrationClient = new TestEntitiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var testEntityId = await dataFactory.CreateTestEntity();

            var command = dataFactory.CreateCommand<UpdateTestEntityCommand>();
            command.Id = testEntityId;

            // Act
            await integrationClient.UpdateTestEntityAsync(testEntityId, command);

            // Assert
            var testEntity = await integrationClient.GetTestEntityByIdAsync(testEntityId);
            Assert.NotNull(testEntity);
            Assert.Equal(command.Name, testEntity.Name);
        }
    }
}