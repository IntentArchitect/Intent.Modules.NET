using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCustomersPagedTests : BaseIntegrationTest
    {
        public GetCustomersPagedTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task GetCustomers_ShouldGetCustomers()
        {
            // Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateCustomer();

            // Act
            var customers2 = await client.GetCustomersAsync();
            var customers = await client.GetCustomersPagedAsync(1, 10);

            // Assert
            Assert.True(customers.Data.Count() > 0);
        }

    }
}