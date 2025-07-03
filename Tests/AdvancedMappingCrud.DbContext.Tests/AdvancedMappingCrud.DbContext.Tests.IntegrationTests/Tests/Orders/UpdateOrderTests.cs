using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests.Orders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateOrderTests : BaseIntegrationTest
    {
        public UpdateOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateOrder_ShouldUpdateOrder()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            var command = dataFactory.CreateCommand<UpdateOrderCommand>();
            command.Id = orderId;

            // Act
            await client.UpdateOrderAsync(orderId, command, TestContext.Current.CancellationToken);

            // Assert
            var order = await client.GetOrderByIdAsync(orderId, TestContext.Current.CancellationToken);
            Assert.NotNull(order);
            Assert.Equal(command.RefNo, order.RefNo);
        }
    }
}