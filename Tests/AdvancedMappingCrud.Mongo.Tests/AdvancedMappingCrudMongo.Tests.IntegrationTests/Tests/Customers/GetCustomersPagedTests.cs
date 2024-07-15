using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCustomersPagedTests : BaseIntegrationTest
    {
        public GetCustomersPagedTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCustomersPaged_ShouldGetCustomersPaged()
        {
            // Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateCustomer();

            // Act
            var customers = await client.GetCustomersPagedAsync(1, 10);

            // Assert
            Assert.True(customers.Data.Count() > 0);
        }

    }
}