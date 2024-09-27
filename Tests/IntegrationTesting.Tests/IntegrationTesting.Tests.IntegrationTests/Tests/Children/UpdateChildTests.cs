using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Children;
using IntegrationTesting.Tests.IntegrationTests.Services.Children;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateChildTests : BaseIntegrationTest
    {
        public UpdateChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateChild_ShouldUpdateChild()
        {
            // Arrange
            var client = new ChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var childId = await dataFactory.CreateChild();

            var command = dataFactory.CreateCommand<UpdateChildCommand>();
            command.Id = childId;

            // Act
            await client.UpdateChildAsync(childId, command);

            // Assert
            var child = await client.GetChildByIdAsync(childId);
            Assert.NotNull(child);
            Assert.Equal(command.Name, child.Name);
        }
    }
}