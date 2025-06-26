using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteCountryTests : BaseIntegrationTest
    {
        public DeleteCountryTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteCountry_ShouldDeleteCountry()
        {
            // Arrange
            var client = new CountriesServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var countryId = await dataFactory.CreateCountry();

            // Act
            await client.DeleteCountryAsync(countryId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.FindCountryByIdAsync(countryId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}