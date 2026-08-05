using System.Net;
using IntegrationTesting.SQLLite.Tests.Api.Controllers.ResponseTypes;
using IntegrationTesting.SQLLite.Tests.Application.Customers;
using IntegrationTesting.SQLLite.Tests.Application.Customers.CreateCustomer;
using IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Harness;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateCustomerTests : BaseIntegrationTest
    {
        public CreateCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateCustomer_WithValidCommand_Returns201WithNewId()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var command = CustomerTestData.BuildCreateCommand(token);

            // Act
            var response = await client.PostAsJsonNoThrowAsync(CustomerTestData.CustomersRoute, command, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.ReadContentAsync<JsonResponse<Guid>>(cancellationToken);
            Assert.NotEqual(Guid.Empty, created.Value);

            var getResponse = await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{created.Value}", cancellationToken);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var customer = await getResponse.ReadContentAsync<CustomerDto>(cancellationToken);
            Assert.Equal(created.Value, customer.Id);
            Assert.Equal(command.FirstName, customer.FirstName);
            Assert.Equal(command.LastName, customer.LastName);
            Assert.Equal(command.Email, customer.Email);
            Assert.Equal(command.PhoneNumber, customer.PhoneNumber);
        }

        [Fact]
        public async Task CreateCustomer_WithoutPhoneNumber_Returns201BecausePhoneNumberIsOptional()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var command = new CreateCustomerCommand(
                firstName: $"First-{token}",
                lastName: $"Last-{token}",
                email: $"{token}@example.com",
                phoneNumber: null);

            // Act
            var response = await client.PostAsJsonNoThrowAsync(CustomerTestData.CustomersRoute, command, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.ReadContentAsync<JsonResponse<Guid>>(cancellationToken);
            var customer = await (await client.GetNoThrowAsync($"{CustomerTestData.CustomersRoute}/{created.Value}", cancellationToken))
                .ReadContentAsync<CustomerDto>(cancellationToken);
            Assert.Null(customer.PhoneNumber);
        }

        [Theory]
        [InlineData("firstName")]
        [InlineData("lastName")]
        [InlineData("email")]
        public async Task CreateCustomer_WithMissingMandatoryField_Returns400(string omittedField)
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = CustomerTestData.NewToken();
            var body = new Dictionary<string, object>
            {
                ["firstName"] = $"First-{token}",
                ["lastName"] = $"Last-{token}",
                ["email"] = $"{token}@example.com"
            };
            body[omittedField] = null;

            // Act
            var response = await client.PostAsJsonNoThrowAsync(CustomerTestData.CustomersRoute, body, cancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorBody = await response.ReadErrorBodyAsync(cancellationToken);
            Assert.Contains(char.ToUpperInvariant(omittedField[0]) + omittedField[1..], errorBody);
        }
    }
}
