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
    public class UpdateCountryTests : BaseIntegrationTest
    {
        public UpdateCountryTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        [IntentIgnore]//Need to fix integration tests with Agg Roots
        public async Task UpdateCountry_ShouldUpdateCountry()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var countryId = await dataFactory.CreateCountry();

            var command = dataFactory.CreateCommand<UpdateCountryDto>();
            //command.Id = countryId;

            // Act
            await client.UpdateCountryAsync(countryId, command);

            // Assert
            var country = await client.FindCountryByIdAsync(countryId);
            Assert.NotNull(country);
            Assert.Equal(command.MaE, country.MaE);
        }
    }
}