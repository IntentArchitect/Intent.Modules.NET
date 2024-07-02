using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Brands;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteBrandTests : BaseIntegrationTest
    {
        public DeleteBrandTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteBrand_ShouldDeleteBrand()
        {
            // Arrange
            var client = new BrandsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var brandId = await dataFactory.CreateBrand();

            // Act
            await client.DeleteBrandAsync(brandId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetBrandByIdAsync(brandId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}