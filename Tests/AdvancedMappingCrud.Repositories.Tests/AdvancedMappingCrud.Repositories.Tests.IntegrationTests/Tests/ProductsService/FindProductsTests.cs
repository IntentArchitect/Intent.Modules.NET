using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class FindProductsTests : BaseIntegrationTest
    {
        public FindProductsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task FindProducts_ShouldFindProducts()
        {
            // Arrange
            var client = new ProductsServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateProduct();

            // Act
            var products = await client.FindProductsAsync();

            // Assert
            Assert.True(products.Count > 0);
        }
    }
}