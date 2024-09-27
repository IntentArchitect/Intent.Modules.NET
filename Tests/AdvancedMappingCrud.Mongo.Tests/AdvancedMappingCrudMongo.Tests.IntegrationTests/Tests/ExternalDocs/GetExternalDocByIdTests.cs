using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.ExternalDocs;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetExternalDocByIdTests : BaseIntegrationTest
    {
        public GetExternalDocByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetExternalDocById_ShouldGetExternalDocById()
        {
            // Arrange
            var client = new ExternalDocsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var externalDocId = await dataFactory.CreateExternalDoc();

            // Act
            var externalDoc = await client.GetExternalDocByIdAsync(externalDocId);

            // Assert
            Assert.NotNull(externalDoc);
        }
    }
}