using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Products;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests.Products
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
            var productId = await client.CreateProductAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var product = await client.GetProductByIdAsync(productId, TestContext.Current.CancellationToken);
            Assert.NotNull(product);
        }
    }
}