using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
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
            var integrationClient = new ProductsServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<ProductCreateDto>();

            // Act
            var productId = await integrationClient.CreateProductAsync(command);

            // Assert
            var product = await integrationClient.FindProductByIdAsync(productId);
            Assert.NotNull(product);
        }
    }
}