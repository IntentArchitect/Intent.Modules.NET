using System.Net;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.ExternalDocs;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteExternalDocTests : BaseIntegrationTest
    {
        public DeleteExternalDocTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteExternalDoc_ShouldDeleteExternalDoc()
        {
            // Arrange
            var client = new ExternalDocsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var externalDocId = await dataFactory.CreateExternalDoc();

            // Act
            await client.DeleteExternalDocAsync(externalDocId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetExternalDocByIdAsync(externalDocId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}