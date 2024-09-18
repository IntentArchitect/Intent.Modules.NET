using System.Net;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.IntegrationTests.HttpClients;
using ValueObjects.Class.IntegrationTests.HttpClients.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace ValueObjects.Class.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteTestEntityTests : BaseIntegrationTest
    {
        public DeleteTestEntityTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteTestEntity_ShouldDeleteTestEntity()
        {
            // Arrange
            var integrationClient = new TestEntitiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var testEntityId = await dataFactory.CreateTestEntity();

            // Act
            await integrationClient.DeleteTestEntityAsync(testEntityId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetTestEntityByIdAsync(testEntityId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}