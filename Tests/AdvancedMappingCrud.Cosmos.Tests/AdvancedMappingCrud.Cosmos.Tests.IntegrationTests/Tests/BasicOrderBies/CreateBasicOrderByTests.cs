using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.BasicOrderBies;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests.BasicOrderBies
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateBasicOrderByTests : BaseIntegrationTest
    {
        public CreateBasicOrderByTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task CreateBasicOrderBy_ShouldCreateBasicOrderBy()
        {
            // Arrange
            var client = new BasicOrderBiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateBasicOrderByCommand>();

            // Act
            var basicOrderById = await client.CreateBasicOrderByAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var basicOrderBy = await client.GetBasicOrderByByIdAsync(basicOrderById, TestContext.Current.CancellationToken);
            Assert.NotNull(basicOrderBy);
        }
    }
}