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
    public class UpdateBrandTests : BaseIntegrationTest
    {
        public UpdateBrandTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateBrand_ShouldUpdateBrand()
        {
            // Arrange
            var client = new BrandsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var brandId = await dataFactory.CreateBrand();

            var command = dataFactory.CreateCommand<UpdateBrandCommand>();
            command.Id = brandId;

            // Act
            await client.UpdateBrandAsync(brandId, command);

            // Assert
            var brand = await client.GetBrandByIdAsync(brandId);
            Assert.NotNull(brand);
            Assert.Equal(command.Name, brand.Name);
        }
    }
}