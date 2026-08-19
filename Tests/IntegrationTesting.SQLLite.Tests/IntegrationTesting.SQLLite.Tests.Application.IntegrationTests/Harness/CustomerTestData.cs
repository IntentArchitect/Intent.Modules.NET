using System.Net;
using IntegrationTesting.SQLLite.Tests.Api.Controllers.ResponseTypes;
using IntegrationTesting.SQLLite.Tests.Application.Customers;
using IntegrationTesting.SQLLite.Tests.Application.Customers.CreateCustomer;

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Harness
{
    /// <summary>
    /// Arranges Customer preconditions through the API itself, so tests never couple to
    /// repositories or the DbContext.
    /// </summary>
    /// <remarks>
    /// The SQLite database is shared across the whole test collection and is NOT reset between
    /// tests, so every customer created here carries a unique token in its searchable fields.
    /// Assertions scope themselves to that token instead of asserting on absolute row counts.
    /// </remarks>
    public static class CustomerTestData
    {
        public const string CustomersRoute = "api/customers";

        /// <summary>
        /// A short, collision-free token used to isolate one test's data from every other test's.
        /// </summary>
        public static string NewToken() => Guid.NewGuid().ToString("N")[..12];

        public static CreateCustomerCommand BuildCreateCommand(string token) => new(
            firstName: $"First-{token}",
            lastName: $"Last-{token}",
            email: $"{token}@example.com",
            phoneNumber: $"+27-{token[..8]}");

        /// <summary>
        /// Creates a customer via the create endpoint and returns its id together with the command
        /// that produced it, so callers can assert against exactly what was sent.
        /// </summary>
        public static async Task<(Guid Id, CreateCustomerCommand Command)> CreateCustomerAsync(
            HttpClient client,
            string token,
            CancellationToken cancellationToken)
        {
            var command = BuildCreateCommand(token);

            var response = await client.PostAsJsonNoThrowAsync(CustomersRoute, command, cancellationToken);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.ReadContentAsync<JsonResponse<Guid>>(cancellationToken);
            Assert.NotEqual(Guid.Empty, created.Value);

            return (created.Value, command);
        }

        /// <summary>
        /// Returns only the customers belonging to <paramref name="token"/>, so a shared database
        /// populated by other tests cannot perturb the assertion.
        /// </summary>
        public static async Task<List<CustomerDto>> GetCustomersForTokenAsync(
            HttpClient client,
            string token,
            CancellationToken cancellationToken)
        {
            var response = await client.GetNoThrowAsync(CustomersRoute, cancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var customers = await response.ReadContentAsync<List<CustomerDto>>(cancellationToken);
            return customers.Where(c => c.Email == $"{token}@example.com").ToList();
        }
    }
}
