using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class FindPagingTSTests : BaseIntegrationTest
    {
        public FindPagingTSTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task FindPagingTS_ShouldFindPagingTS()
        {
            // Arrange
            var client = new PagingTSServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var pagingTSId = await dataFactory.CreatePagingTS();

            // Act
            var pagingTSs = await client.FindPagingTSAsync(1, 10, "Name");

            // Assert
            Assert.NotNull(pagingTSs);
            Assert.True(pagingTSs.Data.Count() > 0);
        }
    }
}