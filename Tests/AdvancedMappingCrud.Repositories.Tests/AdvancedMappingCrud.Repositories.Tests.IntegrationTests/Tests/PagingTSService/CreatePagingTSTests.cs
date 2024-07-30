using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.PagingTS;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreatePagingTSTests : BaseIntegrationTest
    {
        public CreatePagingTSTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreatePagingTS_ShouldCreatePagingTS()
        {
            // Arrange
            var client = new PagingTSServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<PagingTSCreateDto>();

            // Act
            var pagingTSId = await client.CreatePagingTSAsync(command);

            // Assert
            var pagingTS = await client.FindPagingTSByIdAsync(pagingTSId);
            Assert.NotNull(pagingTS);
        }
    }
}