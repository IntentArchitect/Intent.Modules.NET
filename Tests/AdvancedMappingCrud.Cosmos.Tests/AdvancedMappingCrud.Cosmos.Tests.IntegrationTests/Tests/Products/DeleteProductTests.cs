using System.Net;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteProductTests : BaseIntegrationTest
    {
        public DeleteProductTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        /// Filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Category!=ExcludeOnCI
        /// </summary>
        [Fact]
        [Trait("Category", "ExcludeOnCI")]
        public async Task DeleteProduct_ShouldDeleteProduct()
        {
            //Arrange
            var client = new ProductsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var productId = await dataFactory.CreateProduct();

            //Act
            await client.DeleteProductAsync(productId);

            //Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetProductByIdAsync(productId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}