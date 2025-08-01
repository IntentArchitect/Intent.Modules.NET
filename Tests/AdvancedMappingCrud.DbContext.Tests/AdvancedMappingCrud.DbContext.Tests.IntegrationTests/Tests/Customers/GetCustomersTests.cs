using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Customers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCustomersTests : BaseIntegrationTest
    {
        public GetCustomersTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCustomers_ShouldGetCustomers()
        {
            // Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateCustomer();

            // Act
            var customers = await client.GetCustomersAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(customers);
        }
    }
}