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
    public class CreateCityTests : BaseIntegrationTest
    {
        public CreateCityTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateCity_ShouldCreateCity()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateCityDependencies();

            var command = dataFactory.CreateCommand<CreateCityDto>();

            // Act
            var cityId = await client.CreateCityAsync(ids.CountryId, ids.StateId, command, TestContext.Current.CancellationToken);

            // Assert
            var city = await client.FindCityByIdAsync(ids.CountryId, ids.StateId, cityId, TestContext.Current.CancellationToken);
            Assert.NotNull(city);
        }
    }
}