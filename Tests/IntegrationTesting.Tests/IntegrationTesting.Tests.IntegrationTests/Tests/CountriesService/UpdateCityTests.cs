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
    public class UpdateCityTests : BaseIntegrationTest
    {
        public UpdateCityTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateCity_ShouldUpdateCity()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateCity();

            var command = dataFactory.CreateCommand<UpdateCityDto>();

            // Act
            await client.UpdateCityAsync(ids.CountryId, ids.StateId, ids.CityId, command, TestContext.Current.CancellationToken);

            // Assert
            var city = await client.FindCityByIdAsync(ids.CountryId, ids.StateId, ids.CityId, TestContext.Current.CancellationToken);
            Assert.NotNull(city);
            Assert.Equal(command.Name, city.Name);
        }
    }
}