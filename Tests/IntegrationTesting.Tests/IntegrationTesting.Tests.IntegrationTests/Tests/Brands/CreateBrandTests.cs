using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Brands;
using IntegrationTesting.Tests.IntegrationTests.Services.Brands;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateBrandTests : BaseIntegrationTest
    {
        public CreateBrandTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateBrand_ShouldCreateBrand()
        {
            // Arrange
            var integrationClient = new BrandsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateBrandCommand>();

            // Act
            var brandId = await integrationClient.CreateBrandAsync(command);

            // Assert
            var brand = await integrationClient.GetBrandByIdAsync(brandId);
            Assert.NotNull(brand);
        }
    }
}