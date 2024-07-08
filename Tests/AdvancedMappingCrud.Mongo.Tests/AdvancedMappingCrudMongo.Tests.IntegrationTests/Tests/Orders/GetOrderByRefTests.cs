using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOrderByRefTests : BaseIntegrationTest
    {
        public GetOrderByRefTests(IntegrationTestWebAppFactory factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetOrderById_ShouldGetOrderById()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            var creadtedOrder = await client.GetOrderByIdAsync(orderId);
            // Act
            var order = await client.GetOrderByRefAsync(null, creadtedOrder.ExternalRef);

            // Assert
            Assert.NotNull(order);
            Assert.Equal(creadtedOrder.ExternalRef, order.ExternalRef);
        }
    }
}