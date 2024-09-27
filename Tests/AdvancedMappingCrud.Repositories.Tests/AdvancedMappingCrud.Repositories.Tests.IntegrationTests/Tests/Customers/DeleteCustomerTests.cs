using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Customers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteCustomerTests : BaseIntegrationTest
    {
        public DeleteCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteCustomer_ShouldDeleteCustomer()
        {
            // Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            // Act
            await client.DeleteCustomerAsync(customerId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetCustomerByIdAsync(customerId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}