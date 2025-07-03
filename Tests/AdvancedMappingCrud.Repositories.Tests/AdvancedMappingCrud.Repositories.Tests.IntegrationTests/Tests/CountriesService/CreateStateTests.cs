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
    public class CreateStateTests : BaseIntegrationTest
    {
        public CreateStateTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateState_ShouldCreateState()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var countryId = await dataFactory.CreateStateDependencies();

            var command = dataFactory.CreateCommand<CreateStateDto>();

            // Act
            var stateId = await client.CreateStateAsync(countryId, command, TestContext.Current.CancellationToken);

            // Assert
            var state = await client.FindStateByIdAsync(countryId, stateId, TestContext.Current.CancellationToken);
            Assert.NotNull(state);
        }
    }
}