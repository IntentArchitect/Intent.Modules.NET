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
    public class DeleteCheckNewCompChildCrudTests : BaseIntegrationTest
    {
        public DeleteCheckNewCompChildCrudTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteCheckNewCompChildCrud_ShouldDeleteCheckNewCompChildCrud()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var checkNewCompChildCrudId = await dataFactory.CreateCheckNewCompChildCrud();

            // Act
            await client.DeleteCheckNewCompChildCrudAsync(checkNewCompChildCrudId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetCheckNewCompChildCrudByIdAsync(checkNewCompChildCrudId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}