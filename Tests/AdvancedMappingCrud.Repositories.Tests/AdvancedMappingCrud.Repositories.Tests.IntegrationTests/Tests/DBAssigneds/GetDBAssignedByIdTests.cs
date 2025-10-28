using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.DBAssigneds;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.DBAssigneds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDBAssignedByIdTests : BaseIntegrationTest
    {
        public GetDBAssignedByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDBAssignedById_ShouldGetDBAssignedById()
        {
            // Arrange
            var client = new DBAssignedsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var dBAssignedId = await dataFactory.CreateDBAssigned();

            // Act
            var dBAssigned = await client.GetDBAssignedByIdAsync(dBAssignedId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(dBAssigned);
        }
    }
}