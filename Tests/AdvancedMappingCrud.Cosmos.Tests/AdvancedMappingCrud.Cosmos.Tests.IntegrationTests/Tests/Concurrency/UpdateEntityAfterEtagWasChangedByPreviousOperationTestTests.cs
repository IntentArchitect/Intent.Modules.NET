using System.Net;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Concurrency;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateEntityAfterEtagWasChangedByPreviousOperationTestTests : BaseIntegrationTest
    {
        public UpdateEntityAfterEtagWasChangedByPreviousOperationTestTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        /// Filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Category!=ExcludeOnCI
        /// </summary>
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task UpdateEntityAfterEtagWasChangedByPreviousOperationTest_ShouldFailWithConflict()
        {
            //Arrange
            var client = new ConcurrencyHttpClient(CreateClient());

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(async () => await client.UpdateEntityAfterEtagWasChangedByPreviousOperationTestAsync());
            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        }
    }
}