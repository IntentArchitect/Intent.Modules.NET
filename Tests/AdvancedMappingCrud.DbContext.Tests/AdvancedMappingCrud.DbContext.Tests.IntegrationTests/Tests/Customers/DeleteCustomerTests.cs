using System.Net;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Customers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests
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
            var integrationClient = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            // Act
            await integrationClient.DeleteCustomerAsync(customerId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetCustomerByIdAsync(customerId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}