using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Brands;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBrandByIdTests : BaseIntegrationTest
    {
        public GetBrandByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBrandById_ShouldGetBrandById()
        {
            // Arrange
            var client = new BrandsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var brandId = await dataFactory.CreateBrand();

            // Act
            var brand = await client.GetBrandByIdAsync(brandId);

            // Assert
            Assert.NotNull(brand);
        }
    }
}