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
            var integrationClient = new ProductsServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            var command = dataFactory.CreateCommand<ProductUpdateDto>();
            command.Id = productId;

            // Act
            await integrationClient.UpdateProductAsync(productId, command);

            // Assert
            var product = await integrationClient.FindProductByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(command.Name, product.Name);
        }
    }
}