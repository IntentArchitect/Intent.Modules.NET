using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.BasicOrderBies;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateBasicOrderByTests : BaseIntegrationTest
    {
        public UpdateBasicOrderByTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task UpdateBasicOrderBy_ShouldUpdateBasicOrderBy()
        {
            // Arrange
            var client = new BasicOrderBiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var basicOrderById = await dataFactory.CreateBasicOrderBy();

            var command = dataFactory.CreateCommand<UpdateBasicOrderByCommand>();
            command.Id = basicOrderById;

            // Act
            await client.UpdateBasicOrderByAsync(basicOrderById, command);

            // Assert
            var basicOrderBy = await client.GetBasicOrderByByIdAsync(basicOrderById);
            Assert.NotNull(basicOrderBy);
            Assert.Equal(command.Name, basicOrderBy.Name);
        }
    }
}