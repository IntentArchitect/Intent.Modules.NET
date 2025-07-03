using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.CountriesService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteCityTests : BaseIntegrationTest
    {
        public DeleteCityTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteCity_ShouldDeleteCity()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateCity();

            // Act
            await client.DeleteCityAsync(ids.CountryId, ids.StateId, ids.CityId, TestContext.Current.CancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.FindCityByIdAsync(ids.CountryId, ids.StateId, ids.CityId, TestContext.Current.CancellationToken));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}