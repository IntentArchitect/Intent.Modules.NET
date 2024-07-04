using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Products;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateProductTests : BaseIntegrationTest
    {
        public CreateProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateProduct_ShouldCreateProduct()
        {
            // Arrange
            var client = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateProductCommand>();

            // Act
            var productId = await client.CreateProductAsync(command);

            // Assert
            var product = await client.GetProductByIdAsync(productId);
            Assert.NotNull(product);
        }
    }
}