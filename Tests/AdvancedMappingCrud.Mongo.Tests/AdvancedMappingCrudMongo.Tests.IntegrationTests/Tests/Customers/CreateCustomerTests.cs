using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Customers;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Customers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateCustomerTests : BaseIntegrationTest
    {
        public CreateCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateCustomer_ShouldCreateCustomer()
        {
            // Arrange
            var integrationClient = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateCustomerCommand>();

            // Act
            var customerId = await integrationClient.CreateCustomerAsync(command);

            // Assert
            var customer = await integrationClient.GetCustomerByIdAsync(customerId);
            Assert.NotNull(customer);
        }
    }
}