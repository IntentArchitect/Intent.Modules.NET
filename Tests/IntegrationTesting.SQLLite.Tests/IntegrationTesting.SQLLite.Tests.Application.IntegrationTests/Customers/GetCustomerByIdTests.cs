using System.Net;
using IntegrationTesting.SQLLite.Tests.Application.Customers;
using IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Harness;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCustomerByIdTests : BaseIntegrationTest
    {
        public GetCustomerByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCustomerById_ForExistingCustomer_Returns200WithFullDto()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, command) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            // Act
            var response = await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var customer = await response.ReadContentAsync<CustomerDto>(cancellationToken);
            Assert.Equal(customerId, customer.Id);
            Assert.Equal(command.FirstName, customer.FirstName);
            Assert.Equal(command.LastName, customer.LastName);
            Assert.Equal(command.Email, customer.Email);
            Assert.Equal(command.PhoneNumber, customer.PhoneNumber);
        }

        [Fact]
        public async Task GetCustomerById_ForUnknownId_Returns404()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();

            // Act
            var response = await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{Guid.NewGuid()}", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCustomerById_WithMalformedId_Returns400()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();

            // Act
            var response = await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/not-a-guid", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
