using System.Net;
using IntegrationTesting.SQLLite.Tests.Application.Customers;
using IntegrationTesting.SQLLite.Tests.Application.Customers.UpdateCustomer;
using IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Harness;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateCustomerTests : BaseIntegrationTest
    {
        public UpdateCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateCustomer_WithValidCommand_Returns204AndPersistsEveryField()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            var updatedToken = CustomerTestData.NewToken();
            var command = new UpdateCustomerCommand(
                id: customerId,
                firstName: $"First-{updatedToken}",
                lastName: $"Last-{updatedToken}",
                email: $"{updatedToken}@example.com",
                phoneNumber: $"+27-{updatedToken[..8]}");

            // Act
            var response = await client.PutAsJsonNoThrowAsync(
                $"{CustomerTestData.CustomersRoute}/{customerId}",
                command,
                cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var customer = await (await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken))
                .ReadContentAsync<CustomerDto>(cancellationToken);
            Assert.Equal(command.FirstName, customer.FirstName);
            Assert.Equal(command.LastName, customer.LastName);
            Assert.Equal(command.Email, customer.Email);
            Assert.Equal(command.PhoneNumber, customer.PhoneNumber);
        }

        [Fact]
        public async Task UpdateCustomer_WithIdOmittedFromBody_Returns204BecauseTheRouteIdIsUsed()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            var updatedToken = CustomerTestData.NewToken();
            var body = new Dictionary<string, object>
            {
                ["firstName"] = $"First-{updatedToken}",
                ["lastName"] = $"Last-{updatedToken}",
                ["email"] = $"{updatedToken}@example.com"
            };

            // Act
            var response = await client.PutAsJsonNoThrowAsync(
                $"{CustomerTestData.CustomersRoute}/{customerId}",
                body,
                cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var customer = await (await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{customerId}", cancellationToken))
                .ReadContentAsync<CustomerDto>(cancellationToken);
            Assert.Equal((string)body["firstName"], customer.FirstName);
        }

        [Fact]
        public async Task UpdateCustomer_WhenRouteIdAndBodyIdDiffer_Returns400()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            var command = new UpdateCustomerCommand(
                id: Guid.NewGuid(),
                firstName: $"First-{token}",
                lastName: $"Last-{token}",
                email: $"{token}@example.com",
                phoneNumber: null);

            // Act
            var response = await client.PutAsJsonNoThrowAsync(
                $"{CustomerTestData.CustomersRoute}/{customerId}",
                command,
                cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_ForUnknownId_Returns404()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var unknownId = Guid.NewGuid();
            var command = new UpdateCustomerCommand(
                id: unknownId,
                firstName: $"First-{token}",
                lastName: $"Last-{token}",
                email: $"{token}@example.com",
                phoneNumber: null);

            // Act
            var response = await client.PutAsJsonNoThrowAsync(
                $"{CustomerTestData.CustomersRoute}/{unknownId}",
                command,
                cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCustomer_WithMissingMandatoryField_Returns400()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var (customerId, _) = await CustomerTestData.CreateCustomerAsync(client, token, cancellationToken);

            var body = new Dictionary<string, object>
            {
                ["id"] = customerId,
                ["firstName"] = null,
                ["lastName"] = $"Last-{token}",
                ["email"] = $"{token}@example.com"
            };

            // Act
            var response = await client.PutAsJsonNoThrowAsync(
                $"{CustomerTestData.CustomersRoute}/{customerId}",
                body,
                cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorBody = await response.ReadErrorBodyAsync(cancellationToken);
            Assert.Contains("FirstName", errorBody);
        }
    }
}
