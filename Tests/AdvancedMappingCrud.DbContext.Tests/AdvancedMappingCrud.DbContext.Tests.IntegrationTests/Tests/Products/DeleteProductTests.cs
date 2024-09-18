using System.Net;
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
    public class DeleteProductTests : BaseIntegrationTest
    {
        public DeleteProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteProduct_ShouldDeleteProduct()
        {
            // Arrange
            var integrationClient = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            // Act
            await integrationClient.DeleteProductAsync(productId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetProductByIdAsync(productId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}