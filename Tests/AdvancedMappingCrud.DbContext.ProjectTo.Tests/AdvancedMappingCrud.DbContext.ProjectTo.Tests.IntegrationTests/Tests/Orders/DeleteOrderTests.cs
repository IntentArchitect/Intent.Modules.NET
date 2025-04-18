using System.Net;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.HttpClients.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests
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
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            // Act
            await client.DeleteOrderAsync(orderId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetOrderByIdAsync(orderId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}