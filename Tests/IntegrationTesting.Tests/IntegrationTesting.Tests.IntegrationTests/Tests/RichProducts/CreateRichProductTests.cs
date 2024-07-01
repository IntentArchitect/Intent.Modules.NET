using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.RichProducts;
using IntegrationTesting.Tests.IntegrationTests.Services.RichProducts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateRichProductTests : BaseIntegrationTest
    {
        public CreateRichProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateRichProduct_ShouldCreateRichProduct()
        {
            // Arrange
            var client = new RichProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateRichProductDependencies();

            var command = dataFactory.CreateCommand<CreateRichProductCommand>();

            // Act
            var richProductId = await client.CreateRichProductAsync(command);

            // Assert
            var richProduct = await client.GetRichProductByIdAsync(richProductId);
            Assert.NotNull(richProduct);
        }
    }
}