using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.RichProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetRichProductByIdTests : BaseIntegrationTest
    {
        public GetRichProductByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetRichProductById_ShouldGetRichProductById()
        {
            // Arrange
            var client = new RichProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var richProductId = await dataFactory.CreateRichProduct();

            // Act
            var richProduct = await client.GetRichProductByIdAsync(richProductId);

            // Assert
            Assert.NotNull(richProduct);
        }
    }
}