using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeletePagingTSTests : BaseIntegrationTest
    {
        public DeletePagingTSTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeletePagingTS_ShouldDeletePagingTS()
        {
            // Arrange
            var client = new PagingTSServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var pagingTSId = await dataFactory.CreatePagingTS();

            // Act
            await client.DeletePagingTSAsync(pagingTSId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.FindPagingTSByIdAsync(pagingTSId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}