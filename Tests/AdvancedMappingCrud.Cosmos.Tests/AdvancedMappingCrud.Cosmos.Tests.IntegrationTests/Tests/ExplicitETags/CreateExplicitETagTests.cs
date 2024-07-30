using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.ExplicitETags;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateExplicitETagTests : BaseIntegrationTest
    {
        public CreateExplicitETagTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task CreateExplicitETag_ShouldCreateExplicitETag()
        {
            // Arrange
            var client = new ExplicitETagsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateExplicitETagCommand>();

            // Act
            var explicitETagId = await client.CreateExplicitETagAsync(command);

            // Assert
            var explicitETag = await client.GetExplicitETagByIdAsync(explicitETagId);
            Assert.NotNull(explicitETag);
        }
    }
}