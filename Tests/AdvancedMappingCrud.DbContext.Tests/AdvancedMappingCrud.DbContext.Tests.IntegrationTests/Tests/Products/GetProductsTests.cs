using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetProductsTests : BaseIntegrationTest
    {
        public GetProductsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetProducts_ShouldGetProducts()
        {
            // Arrange
            var client = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateProduct();

            // Act
            var products = await client.GetProductsAsync();

            // Assert
            Assert.True(products.Count > 0);
        }
    }
}