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
    public class CreateOrderTests : BaseIntegrationTest
    {
        public CreateOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateOrder_ShouldCreateOrder()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateOrderDependencies();

            var command = dataFactory.CreateCommand<CreateOrderCommand>();

            // Act
            var orderId = await client.CreateOrderAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var order = await client.GetOrderByIdAsync(orderId, TestContext.Current.CancellationToken);
            Assert.NotNull(order);
        }
    }
}