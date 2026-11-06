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
    public class SearchCustomersTests : BaseIntegrationTest
    {
        private const string SearchRoute = "api/customers/search";

        public SearchCustomersTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task SearchCustomers_OverHttpQuery_ReturnsOnlyTheMatchingCustomers()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (firstId, command) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);
            var (secondId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);
            var (otherId, _) = await CustomerTestData.CreateCustomerAsync(client, CustomerTestData.NewToken(), cancellationToken);

            // Act
            var response = await client.QueryAsJsonNoThrowAsync(SearchRoute, new { command.LastName }, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var customers = await response.ReadContentAsync<List<CustomerDto>>(cancellationToken);
            Assert.Equal(2, customers.Count);
            Assert.Contains(customers, c => c.Id == firstId);
            Assert.Contains(customers, c => c.Id == secondId);
            Assert.DoesNotContain(customers, c => c.Id == otherId);
        }

        [Fact]
        public async Task SearchCustomers_WithPost_Returns405()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();

            // Act
            var response = await client.PostAsJsonNoThrowAsync(SearchRoute, new { LastName = "Anyone" }, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
}
