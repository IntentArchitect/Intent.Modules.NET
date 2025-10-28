using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.DBAssigneds;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.DBAssigneds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteDBAssignedTests : BaseIntegrationTest
    {
        public DeleteDBAssignedTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteDBAssigned_ShouldDeleteDBAssigned()
        {
            // Arrange
            var client = new DBAssignedsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var dBAssignedId = await dataFactory.CreateDBAssigned();

            // Act
            await client.DeleteDBAssignedAsync(dBAssignedId, TestContext.Current.CancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetDBAssignedByIdAsync(dBAssignedId, TestContext.Current.CancellationToken));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}