using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Parents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetParentByIdTests : BaseIntegrationTest
    {
        public GetParentByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetParentById_ShouldGetParentById()
        {
            // Arrange
            var client = new ParentsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var parentId = await dataFactory.CreateParent();

            // Act
            var parent = await client.GetParentByIdAsync(parentId);

            // Assert
            Assert.NotNull(parent);
        }
    }
}