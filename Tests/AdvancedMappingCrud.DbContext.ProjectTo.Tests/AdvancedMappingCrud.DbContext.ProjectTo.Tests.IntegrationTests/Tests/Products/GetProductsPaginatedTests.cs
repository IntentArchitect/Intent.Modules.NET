using AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.HttpClients.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetProductsPaginatedTests : BaseIntegrationTest
    {
        public GetProductsPaginatedTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetProductsPaginatedByNameOptionalTests_ShouldGetProducts()
        {
            // Arrange
            var client = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateProduct();
            await dataFactory.CreateProduct();

            // Act
            var products = await client.GetProductsPaginatedAsync(1, 2);

            // Assert
            Assert.True(products.Data.Count > 0);
        }

    }
}