using System.Net;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.SimpleOdata;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests.SimpleOdata
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteSimpleOdataTests : BaseIntegrationTest
    {
        public DeleteSimpleOdataTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task DeleteSimpleOdata_ShouldDeleteSimpleOdata()
        {
            // Arrange
            var client = new SimpleOdataHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var simpleOdataId = await dataFactory.CreateSimpleOdata();

            // Act
            await client.DeleteSimpleOdataAsync(simpleOdataId, TestContext.Current.CancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetSimpleOdataByIdAsync(simpleOdataId, TestContext.Current.CancellationToken));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}