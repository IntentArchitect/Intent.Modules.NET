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
    public class GetCustomersTests : BaseIntegrationTest
    {
        public GetCustomersTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCustomers_Returns200IncludingTheCustomerJustCreated()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, command) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            // Act
            var response = await client.GetNoThrowAsync(CustomerTestData.CustomersRoute, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var customers = await response.ReadContentAsync<List<CustomerDto>>(cancellationToken);

            // The database is shared across the collection, so scope to this test's own token
            // rather than asserting on the total number of rows.
            var customer = Assert.Single(customers, c => c.Id == customerId);
            Assert.Equal(command.FirstName, customer.FirstName);
            Assert.Equal(command.LastName, customer.LastName);
            Assert.Equal(command.Email, customer.Email);
        }

        [Fact]
        public async Task GetCustomers_AfterCreatingTwo_ReturnsBothForThatToken()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (firstId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);
            var (secondId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            // Act
            var matches = await CustomerTestData.GetCustomersForTokenAsync(client, token, cancellationToken);

            // Assert
            Assert.Equal(2, matches.Count);
            Assert.Contains(matches, c => c.Id == firstId);
            Assert.Contains(matches, c => c.Id == secondId);
        }
    }
}
