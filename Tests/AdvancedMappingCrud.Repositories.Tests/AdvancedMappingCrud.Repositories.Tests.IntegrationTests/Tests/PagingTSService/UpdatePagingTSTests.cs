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
    public class UpdatePagingTSTests : BaseIntegrationTest
    {
        public UpdatePagingTSTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdatePagingTS_ShouldUpdatePagingTS()
        {
            // Arrange
            var client = new PagingTSServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var pagingTSId = await dataFactory.CreatePagingTS();

            var command = dataFactory.CreateCommand<PagingTSUpdateDto>();
            command.Id = pagingTSId;

            // Act
            await client.UpdatePagingTSAsync(pagingTSId, command);

            // Assert
            var pagingTS = await client.FindPagingTSByIdAsync(pagingTSId);
            Assert.NotNull(pagingTS);
            Assert.Equal(command.Name, pagingTS.Name);
        }
    }
}