using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Children;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteChildTests : BaseIntegrationTest
    {
        public DeleteChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteChild_ShouldDeleteChild()
        {
            // Arrange
            var integrationClient = new ChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var childId = await dataFactory.CreateChild();

            // Act
            await integrationClient.DeleteChildAsync(childId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetChildByIdAsync(childId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}