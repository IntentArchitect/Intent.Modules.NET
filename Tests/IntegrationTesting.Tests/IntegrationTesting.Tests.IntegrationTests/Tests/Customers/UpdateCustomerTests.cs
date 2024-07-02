using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Customers;
using IntegrationTesting.Tests.IntegrationTests.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateCustomerTests : BaseIntegrationTest
    {
        public UpdateCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateCustomer_ShouldUpdateCustomer()
        {
            // Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            var command = dataFactory.CreateCommand<UpdateCustomerCommand>();
            command.Id = customerId;

            // Act
            await client.UpdateCustomerAsync(customerId, command);

            // Assert
            var customer = await client.GetCustomerByIdAsync(customerId);
            Assert.NotNull(customer);
            Assert.Equal(command.Name, customer.Name);
        }
    }
}