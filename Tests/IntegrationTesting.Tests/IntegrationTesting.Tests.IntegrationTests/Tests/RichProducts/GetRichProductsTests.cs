using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.RichProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetRichProductsTests : BaseIntegrationTest
    {
        public GetRichProductsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetRichProducts_ShouldGetRichProducts()
        {
            // Arrange
            var client = new RichProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateRichProduct();

            // Act
            var richProducts = await client.GetRichProductsAsync();

            // Assert
            Assert.True(richProducts.Count > 0);
        }
    }
}