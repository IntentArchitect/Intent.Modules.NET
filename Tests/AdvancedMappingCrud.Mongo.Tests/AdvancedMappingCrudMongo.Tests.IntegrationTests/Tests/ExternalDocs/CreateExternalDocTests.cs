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
    public class CreateExternalDocTests : BaseIntegrationTest
    {
        public CreateExternalDocTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        [IntentMerge]
        public async Task CreateExternalDoc_ShouldCreateExternalDoc()
        {
            // Arrange
            var client = new ExternalDocsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateExternalDocCommand>();
            command.Id = UniqueLongGenerator.GetNextId();

            // Act
            var externalDocId = await client.CreateExternalDocAsync(command);

            // Assert
            var externalDoc = await client.GetExternalDocByIdAsync(externalDocId);
            Assert.NotNull(externalDoc);
        }
    }
}