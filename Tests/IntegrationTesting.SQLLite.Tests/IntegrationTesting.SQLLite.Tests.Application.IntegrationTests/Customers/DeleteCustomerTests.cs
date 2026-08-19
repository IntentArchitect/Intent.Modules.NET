using System.Net;
using IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Harness;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteCustomerTests : BaseIntegrationTest
    {
        public DeleteCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteCustomer_ForExistingCustomer_Returns200AndRemovesIt()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            // Act
            var response = await client.DeleteNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

            var remaining = await CustomerTestData.GetCustomersForTokenAsync(client, token, cancellationToken);
            Assert.Empty(remaining);
        }

        [Fact]
        public async Task DeleteCustomer_ForUnknownId_Returns404()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();

            // Act
            var response = await client.DeleteNoThrowAsync($"{CustomerTestData.CustomersRoute}/{Guid.NewGuid()}", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_CalledTwice_ReturnsNotFoundOnTheSecondCall()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);
            var firstDelete = await client.DeleteNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken);
            Assert.Equal(HttpStatusCode.OK, firstDelete.StatusCode);

            // Act
            var secondDelete = await client.DeleteNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, secondDelete.StatusCode);
        }
    }
}
