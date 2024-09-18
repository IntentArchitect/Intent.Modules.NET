using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCustomerByIdTests : BaseIntegrationTest
    {
        public GetCustomerByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCustomerById_ShouldGetCustomerById()
        {
            // Arrange
            var integrationClient = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            // Act
            var customer = await integrationClient.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.NotNull(customer);
        }
    }
}