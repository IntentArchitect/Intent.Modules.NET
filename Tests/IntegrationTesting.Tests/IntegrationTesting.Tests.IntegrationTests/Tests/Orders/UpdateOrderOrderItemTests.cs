using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Orders;
using IntegrationTesting.Tests.IntegrationTests.Services.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.Orders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateOrderOrderItemTests : BaseIntegrationTest
    {
        public UpdateOrderOrderItemTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateOrderOrderItem_ShouldUpdateOrderOrderItem()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateOrderItem();

            var command = dataFactory.CreateCommand<UpdateOrderOrderItemCommand>();
            command.Id = ids.OrderItemId;

            // Act
            await client.UpdateOrderOrderItemAsync(ids.OrderItemId, command, TestContext.Current.CancellationToken);

            // Assert
            var orderItem = await client.GetOrderOrderItemByIdAsync(ids.OrderId, ids.OrderItemId, TestContext.Current.CancellationToken);
            Assert.NotNull(orderItem);
            Assert.Equal(command.Description, orderItem.Description);
        }
    }
}