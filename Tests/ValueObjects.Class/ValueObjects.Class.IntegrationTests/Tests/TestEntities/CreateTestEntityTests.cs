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
    public class CreateTestEntityTests : BaseIntegrationTest
    {
        public CreateTestEntityTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateTestEntity_ShouldCreateTestEntity()
        {
            // Arrange
            var client = new TestEntitiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateTestEntityCommand>();

            // Act
            var testEntityId = await client.CreateTestEntityAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var testEntity = await client.GetTestEntityByIdAsync(testEntityId, TestContext.Current.CancellationToken);
            Assert.NotNull(testEntity);
        }
    }
}