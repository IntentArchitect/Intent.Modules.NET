using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteOrderTests : BaseIntegrationTest
    {
        public DeleteOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteOrder_ShouldDeleteOrder()
        {
            // Arrange
            var integrationClient = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            // Act
            await integrationClient.DeleteOrderAsync(orderId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetOrderByIdAsync(orderId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}