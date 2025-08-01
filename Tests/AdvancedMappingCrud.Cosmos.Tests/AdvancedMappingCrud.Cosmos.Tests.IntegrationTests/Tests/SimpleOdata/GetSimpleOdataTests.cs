using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.SimpleOdata;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests.SimpleOdata
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetSimpleOdataTests : BaseIntegrationTest
    {
        public GetSimpleOdataTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task GetSimpleOdata_ShouldGetSimpleOdata()
        {
            // Arrange
            var client = new SimpleOdataHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateSimpleOdata();

            // Act
            var simpleOdata = await client.GetSimpleOdataAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(simpleOdata);
        }
    }
}