using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Customers;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Customers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests.Customers
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
            await client.UpdateCustomerAsync(customerId, command, TestContext.Current.CancellationToken);

            // Assert
            var customer = await client.GetCustomerByIdAsync(customerId, TestContext.Current.CancellationToken);
            Assert.NotNull(customer);
            Assert.Equal(command.Name, customer.Name);
        }
    }
}