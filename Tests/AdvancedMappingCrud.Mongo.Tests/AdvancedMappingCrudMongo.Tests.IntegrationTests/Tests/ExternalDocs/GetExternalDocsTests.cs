using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.ExternalDocs;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetExternalDocsTests : BaseIntegrationTest
    {
        public GetExternalDocsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetExternalDocs_ShouldGetExternalDocs()
        {
            // Arrange
            var client = new ExternalDocsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateExternalDoc();

            // Act
            var externalDocs = await client.GetExternalDocsAsync();

            // Assert
            Assert.True(externalDocs.Count > 0);
        }
    }
}