using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Orders;
using IntegrationTesting.Tests.IntegrationTests.Services.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
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
            var orderId = await client.CreateOrderAsync(command);

            // Assert
            var order = await client.GetOrderByIdAsync(orderId);
            Assert.NotNull(order);
        }
    }
}