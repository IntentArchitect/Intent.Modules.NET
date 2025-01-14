using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.CheckNewCompChildCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteCNCCChildTests : BaseIntegrationTest
    {
        public DeleteCNCCChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteCNCCChild_ShouldDeleteCNCCChild()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateCNCCChild();

            // Act
            await client.DeleteCNCCChildAsync(ids.CheckNewCompChildCrudId, ids.CNCCChildId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetCNCCChildByIdAsync(ids.CheckNewCompChildCrudId, ids.CNCCChildId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}