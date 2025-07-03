using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.CountriesService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateStateTests : BaseIntegrationTest
    {
        public UpdateStateTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        [IntentIgnore]//Need to fix integration tests with Agg Roots
        public async Task UpdateState_ShouldUpdateState()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateState();

            var command = dataFactory.CreateCommand<UpdateStateDto>();
            //command.Id = ids.StateId;

            // Act
            await client.UpdateStateAsync(ids.CountryId, ids.StateId, command);

            // Assert
            var state = await client.FindStateByIdAsync(ids.CountryId, ids.StateId);
            Assert.NotNull(state);
            Assert.Equal(command.Name, state.Name);
        }
    }
}