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
    public class UpdateCountryTests : BaseIntegrationTest
    {
        public UpdateCountryTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateCountry_ShouldUpdateCountry()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var countryId = await dataFactory.CreateCountry();

            var command = dataFactory.CreateCommand<UpdateCountryDto>();

            // Act
            await client.UpdateCountryAsync(countryId, command, TestContext.Current.CancellationToken);

            // Assert
            var country = await client.FindCountryByIdAsync(countryId, TestContext.Current.CancellationToken);
            Assert.NotNull(country);
            Assert.Equal(command.Name, country.Name);
        }
    }
}