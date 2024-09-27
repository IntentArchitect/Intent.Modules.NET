using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Children;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetChildByIdTests : BaseIntegrationTest
    {
        public GetChildByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetChildById_ShouldGetChildById()
        {
            // Arrange
            var client = new ChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var childId = await dataFactory.CreateChild();

            // Act
            var child = await client.GetChildByIdAsync(childId);

            // Assert
            Assert.NotNull(child);
        }
    }
}