using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.DBAssigneds;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.DBAssigneds;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.DBAssigneds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateDBAssignedTests : BaseIntegrationTest
    {
        public CreateDBAssignedTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateDBAssigned_ShouldCreateDBAssigned()
        {
            // Arrange
            var client = new DBAssignedsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateDBAssignedCommand>();

            // Act
            var dBAssignedId = await client.CreateDBAssignedAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var dBAssigned = await client.GetDBAssignedByIdAsync(dBAssignedId, TestContext.Current.CancellationToken);
            Assert.NotNull(dBAssigned);
        }
    }
}