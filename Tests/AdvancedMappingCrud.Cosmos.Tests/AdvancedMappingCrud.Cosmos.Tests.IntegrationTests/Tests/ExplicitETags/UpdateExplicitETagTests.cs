using System.Net;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
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
    public class UpdateExplicitETagTests : BaseIntegrationTest
    {
        public UpdateExplicitETagTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        [IntentIgnore]
        public async Task UpdateExplicitETag_ShouldUpdateExplicitETag()
        {
            // Arrange
            var client = new ExplicitETagsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var explicitETagId = await dataFactory.CreateExplicitETag();

            var command = dataFactory.CreateCommand<UpdateExplicitETagCommand>();
            command.Id = explicitETagId;


            var getCreated = await client.GetExplicitETagByIdAsync(explicitETagId);
            //Set ETag to be correct value
            command.ETag = getCreated.ETag;
            // Act
            await client.UpdateExplicitETagAsync(explicitETagId, command);

            // Assert
            var explicitETag = await client.GetExplicitETagByIdAsync(explicitETagId);
            Assert.NotNull(explicitETag);
            Assert.Equal(command.Name, explicitETag.Name);
        }

        [Fact]
        [Trait("Requirement", "CosmosDB")]
        [IntentIgnore]
        public async Task UpdateExplicitETag_ShouldFailWithWrongETag()
        {
            // Arrange
            var client = new ExplicitETagsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var explicitETagId = await dataFactory.CreateExplicitETag();

            var command = dataFactory.CreateCommand<UpdateExplicitETagCommand>();
            command.Id = explicitETagId;


            var getCreated = await client.GetExplicitETagByIdAsync(explicitETagId);
            //Set ETag to be correct value
            command.ETag = Guid.NewGuid().ToString() + "Diff";
            // Act
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.UpdateExplicitETagAsync(explicitETagId, command));

            // Assert

            Assert.NotNull(exception);
        }

    }
}