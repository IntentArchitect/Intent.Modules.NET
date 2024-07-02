using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DiffIds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDiffIdByIdTests : BaseIntegrationTest
    {
        public GetDiffIdByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDiffIdById_ShouldGetDiffIdById()
        {
            // Arrange
            var client = new DiffIdsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var diffIdId = await dataFactory.CreateDiffId();

            // Act
            var diffId = await client.GetDiffIdByIdAsync(diffIdId);

            // Assert
            Assert.NotNull(diffId);
        }
    }
}