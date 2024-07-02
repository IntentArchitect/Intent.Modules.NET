using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class FindProductByIdTests : BaseIntegrationTest
    {
        public FindProductByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task FindProductById_ShouldFindProductById()
        {
            // Arrange
            var client = new ProductsServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            // Act
            var product = await client.FindProductByIdAsync(productId);

            // Assert
            Assert.NotNull(product);
        }
    }
}