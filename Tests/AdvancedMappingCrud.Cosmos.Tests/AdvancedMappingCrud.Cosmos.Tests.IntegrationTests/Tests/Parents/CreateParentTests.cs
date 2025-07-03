using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Parents;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests.Parents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateParentTests : BaseIntegrationTest
    {
        public CreateParentTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// You can use this trait to filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Requirement!="CosmosDB"
        /// </summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task CreateParent_ShouldCreateParent()
        {
            // Arrange
            var client = new ParentsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateParentCommand>();

            // Act
            var parentId = await client.CreateParentAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var parent = await client.GetParentByIdAsync(parentId, TestContext.Current.CancellationToken);
            Assert.NotNull(parent);
        }
    }
}