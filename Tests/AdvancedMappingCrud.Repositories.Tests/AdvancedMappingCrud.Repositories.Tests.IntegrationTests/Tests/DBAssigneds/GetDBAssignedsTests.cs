using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.DBAssigneds;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.DBAssigneds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDBAssignedsTests : BaseIntegrationTest
    {
        public GetDBAssignedsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDBAssigneds_ShouldGetDBAssigneds()
        {
            // Arrange
            var client = new DBAssignedsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateDBAssigned();

            // Act
            var dBAssigneds = await client.GetDBAssignedsAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(dBAssigneds);
        }
    }
}