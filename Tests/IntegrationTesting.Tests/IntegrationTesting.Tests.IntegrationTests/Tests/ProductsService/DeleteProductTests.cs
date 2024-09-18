using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
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
            var integrationClient = new ProductsServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            // Act
            await integrationClient.DeleteProductAsync(productId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.FindProductByIdAsync(productId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}