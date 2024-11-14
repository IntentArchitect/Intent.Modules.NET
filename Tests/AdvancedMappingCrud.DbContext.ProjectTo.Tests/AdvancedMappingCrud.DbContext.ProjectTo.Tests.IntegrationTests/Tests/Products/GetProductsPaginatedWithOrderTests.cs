using AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.HttpClients.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetProductsPaginatedWithOrderTests : BaseIntegrationTest
    {
        public GetProductsPaginatedWithOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }


        [Fact]
        public async Task GetProductsPaginatedWithOrderAsync_ShouldGetProducts()
        {
            // Arrange
            var client = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateProduct();
            await dataFactory.CreateProduct();

            // Act
            var products = await client.GetProductsPaginatedWithOrderAsync(1, 2, "Name");

            // Assert
            Assert.True(products.Data.Count > 0);
            Assert.True(products.Data[0].Name.CompareTo(products.Data[1].Name) <= 0);
        }
    }
}