using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.Services.Countries;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.CountriesService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateStateTests : BaseIntegrationTest
    {
        public UpdateStateTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateState_ShouldUpdateState()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateState();

            var command = dataFactory.CreateCommand<UpdateStateDto>();

            // Act
            await client.UpdateStateAsync(ids.CountryId, ids.StateId, command, TestContext.Current.CancellationToken);

            // Assert
            var state = await client.FindStateByIdAsync(ids.CountryId, ids.StateId, TestContext.Current.CancellationToken);
            Assert.NotNull(state);
            Assert.Equal(command.Name, state.Name);
        }
    }
}