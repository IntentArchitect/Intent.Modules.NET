using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests
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
            await client.UpdateOrderOrderItemAsync(ids.OrderItemId, command);

            // Assert
            var orderItem = await client.GetOrderOrderItemByIdAsync(ids.OrderId, ids.OrderItemId);
            Assert.NotNull(orderItem);
            Assert.Equal(command.Quantity, orderItem.Quantity);
        }
    }
}