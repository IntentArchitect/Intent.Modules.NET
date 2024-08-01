using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class FindPagingTSByIdTests : BaseIntegrationTest
    {
        public FindPagingTSByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task FindPagingTSById_ShouldFindPagingTSById()
        {
            // Arrange
            var client = new PagingTSServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var pagingTSId = await dataFactory.CreatePagingTS();

            // Act
            var pagingTS = await client.FindPagingTSByIdAsync(pagingTSId);

            // Assert
            Assert.NotNull(pagingTS);
        }
    }
}