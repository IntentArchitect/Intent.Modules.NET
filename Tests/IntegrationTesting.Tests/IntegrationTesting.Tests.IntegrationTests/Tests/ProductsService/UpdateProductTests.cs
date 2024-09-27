using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateProductTests : BaseIntegrationTest
    {
        public UpdateProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateProduct_ShouldUpdateProduct()
        {
            // Arrange
            var client = new ProductsServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            var command = dataFactory.CreateCommand<ProductUpdateDto>();
            command.Id = productId;

            // Act
            await client.UpdateProductAsync(productId, command);

            // Assert
            var product = await client.FindProductByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(command.Name, product.Name);
        }
    }
}