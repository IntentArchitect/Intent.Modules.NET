using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DiffIds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteDiffIdTests : BaseIntegrationTest
    {
        public DeleteDiffIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteDiffId_ShouldDeleteDiffId()
        {
            // Arrange
            var integrationClient = new DiffIdsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var diffIdId = await dataFactory.CreateDiffId();

            // Act
            await integrationClient.DeleteDiffIdAsync(diffIdId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetDiffIdByIdAsync(diffIdId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}