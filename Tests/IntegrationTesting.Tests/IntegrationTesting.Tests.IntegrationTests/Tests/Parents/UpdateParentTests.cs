using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Parents;
using IntegrationTesting.Tests.IntegrationTests.Services.Parents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateParentTests : BaseIntegrationTest
    {
        public UpdateParentTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateParent_ShouldUpdateParent()
        {
            // Arrange
            var integrationClient = new ParentsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var parentId = await dataFactory.CreateParent();

            var command = dataFactory.CreateCommand<UpdateParentCommand>();
            command.Id = parentId;

            // Act
            await integrationClient.UpdateParentAsync(parentId, command);

            // Assert
            var parent = await integrationClient.GetParentByIdAsync(parentId);
            Assert.NotNull(parent);
            Assert.Equal(command.Name, parent.Name);
        }
    }
}