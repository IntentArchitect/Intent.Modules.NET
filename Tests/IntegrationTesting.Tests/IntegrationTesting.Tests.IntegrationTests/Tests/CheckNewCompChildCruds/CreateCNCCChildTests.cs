using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.CheckNewCompChildCruds;
using IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateCNCCChildTests : BaseIntegrationTest
    {
        public CreateCNCCChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateCNCCChild_ShouldCreateCNCCChild()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var checkNewCompChildCrudId = await dataFactory.CreateCNCCChildDependencies();

            var command = dataFactory.CreateCommand<CreateCNCCChildCommand>();

            // Act
            var cNCCChildId = await client.CreateCNCCChildAsync(checkNewCompChildCrudId, command);

            // Assert
            var cNCCChild = await client.GetCNCCChildByIdAsync(checkNewCompChildCrudId, cNCCChildId);
            Assert.NotNull(cNCCChild);
        }
    }
}