using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.IntegrationTests.HttpClients.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace ValueObjects.Record.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetTestEntitiesTests : BaseIntegrationTest
    {
        public GetTestEntitiesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetTestEntities_ShouldGetTestEntities()
        {
            // Arrange
            var client = new TestEntitiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateTestEntity();

            // Act
            var testEntities = await client.GetTestEntitiesAsync();

            // Assert
            Assert.True(testEntities.Count > 0);
        }
    }
}