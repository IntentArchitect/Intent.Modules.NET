using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.IntegrationTests.HttpClients.TestEntities;
using ValueObjects.Class.IntegrationTests.Services.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace ValueObjects.Class.IntegrationTests.Tests.TestEntities
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
            var client = new TestEntitiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var testEntityId = await dataFactory.CreateTestEntity();

            var command = dataFactory.CreateCommand<UpdateTestEntityCommand>();
            command.Id = testEntityId;

            // Act
            await client.UpdateTestEntityAsync(testEntityId, command, TestContext.Current.CancellationToken);

            // Assert
            var testEntity = await client.GetTestEntityByIdAsync(testEntityId, TestContext.Current.CancellationToken);
            Assert.NotNull(testEntity);
            Assert.Equal(command.Name, testEntity.Name);
        }
    }
}