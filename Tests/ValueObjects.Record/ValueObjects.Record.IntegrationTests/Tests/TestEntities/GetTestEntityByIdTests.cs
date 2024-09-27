using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.IntegrationTests.HttpClients.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace ValueObjects.Record.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetTestEntityByIdTests : BaseIntegrationTest
    {
        public GetTestEntityByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetTestEntityById_ShouldGetTestEntityById()
        {
            // Arrange
            var client = new TestEntitiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var testEntityId = await dataFactory.CreateTestEntity();

            // Act
            var testEntity = await client.GetTestEntityByIdAsync(testEntityId);

            // Assert
            Assert.NotNull(testEntity);
        }
    }
}