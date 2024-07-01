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
    public class UpdateRichProductTests : BaseIntegrationTest
    {
        public UpdateRichProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateRichProduct_ShouldUpdateRichProduct()
        {
            // Arrange
            var client = new RichProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var richProductId = await dataFactory.CreateRichProduct();

            var command = dataFactory.CreateCommand<UpdateRichProductCommand>();
            command.Id = richProductId;

            // Act
            await client.UpdateRichProductAsync(richProductId, command);

            // Assert
            var richProduct = await client.GetRichProductByIdAsync(richProductId);
            Assert.NotNull(richProduct);
            Assert.Equal(command.Name, richProduct.Name);
        }
    }
}