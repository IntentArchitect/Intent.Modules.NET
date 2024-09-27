using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Brands;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBrandsTests : BaseIntegrationTest
    {
        public GetBrandsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBrands_ShouldGetBrands()
        {
            // Arrange
            var client = new BrandsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateBrand();

            // Act
            var brands = await client.GetBrandsAsync();

            // Assert
            Assert.True(brands.Count > 0);
        }
    }
}