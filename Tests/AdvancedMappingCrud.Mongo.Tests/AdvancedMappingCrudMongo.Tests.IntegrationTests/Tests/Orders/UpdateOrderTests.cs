using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateOrderTests : BaseIntegrationTest
    {
        public UpdateOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        [IntentIgnore]
        public async Task UpdateOrder_ShouldUpdateOrder()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            var command = dataFactory.CreateCommand<UpdateOrderCommand>();
            command.Id = orderId;

            var createdOrder = await client.GetOrderByIdAsync(orderId);

            for (var i = 0; i < createdOrder.OrderItems.Count; i++)
            {
                command.OrderItems[i].Id = createdOrder.OrderItems[i].Id;
            }

            // Act
            await client.UpdateOrderAsync(orderId, command);

            // Assert
            var order = await client.GetOrderByIdAsync(orderId);
            Assert.NotNull(order);
            Assert.Equal(command.RefNo, order.RefNo);
        }
    }
}