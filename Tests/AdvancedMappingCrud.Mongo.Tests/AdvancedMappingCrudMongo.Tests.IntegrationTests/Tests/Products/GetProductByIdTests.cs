using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetProductByIdTests : BaseIntegrationTest
    {
        public GetProductByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetProductById_ShouldGetProductById()
        {
            // Arrange
            var client = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            // Act
            var product = await client.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(product);
        }
    }
}