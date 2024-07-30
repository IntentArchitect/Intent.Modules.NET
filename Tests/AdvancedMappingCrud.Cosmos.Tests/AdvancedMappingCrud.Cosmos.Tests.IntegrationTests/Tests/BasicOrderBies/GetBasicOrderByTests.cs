using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.BasicOrderBies;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBasicOrderByTests : BaseIntegrationTest
    {
        public GetBasicOrderByTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task GetBasicOrderBy_ShouldGetBasicOrderBy()
        {
            // Arrange
            var client = new BasicOrderBiesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateBasicOrderBy();

            // Act
            var customers = await client.GetBasicOrderByAsync(1, 10, "Name asc");

            // Assert
            Assert.True(customers.Data.Count() > 0);
        }
    }
}