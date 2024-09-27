using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.ExternalDocs;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.ExternalDocs;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateExternalDocTests : BaseIntegrationTest
    {
        public UpdateExternalDocTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateExternalDoc_ShouldUpdateExternalDoc()
        {
            // Arrange
            var client = new ExternalDocsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var externalDocId = await dataFactory.CreateExternalDoc();

            var command = dataFactory.CreateCommand<UpdateExternalDocCommand>();
            command.Id = externalDocId;

            // Act
            await client.UpdateExternalDocAsync(externalDocId, command);

            // Assert
            var externalDoc = await client.GetExternalDocByIdAsync(externalDocId);
            Assert.NotNull(externalDoc);
            Assert.Equal(command.Name, externalDoc.Name);
        }
    }
}