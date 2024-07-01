using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.RichProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteRichProductTests : BaseIntegrationTest
    {
        public DeleteRichProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteRichProduct_ShouldDeleteRichProduct()
        {
            // Arrange
            var client = new RichProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var richProductId = await dataFactory.CreateRichProduct();

            // Act
            await client.DeleteRichProductAsync(richProductId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetRichProductByIdAsync(richProductId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}