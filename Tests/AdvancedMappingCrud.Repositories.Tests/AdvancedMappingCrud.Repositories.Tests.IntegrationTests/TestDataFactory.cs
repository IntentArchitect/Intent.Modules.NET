using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests
{
    public class TestDataFactory
    {
        private readonly Fixture _fixture;
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly Dictionary<string, object> _idTracker = new();

        public TestDataFactory(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
            _fixture = new Fixture();
        }

        public T CreateCommand<T>()
        {
            var fixture = new Fixture();
            fixture.RepeatCount = 1;
            fixture.Customizations.Add(new PopulateIdsSpecimenBuilder(_idTracker));
            return fixture.Create<T>();
        }

        public async Task<Guid> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }

        public async Task CreateOrderDependencies()
        {
            await CreateCustomer();
            await CreateProduct();
        }

        public async Task<Guid> CreateOrder()
        {
            await CreateOrderDependencies();

            var client = new OrdersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateOrderCommand>();
            var orderId = await client.CreateOrderAsync(command);
            _idTracker["OrderId"] = orderId;
            return orderId;
        }

        public async Task<Guid> CreateOrderItemDependencies()
        {
            var orderId = await CreateOrder();
            await CreateProduct();
            return orderId;
        }

        public async Task<(Guid OrderId, Guid OrderItemId)> CreateOrderItem()
        {
            var orderId = await CreateOrderItemDependencies();

            var client = new OrdersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateOrderOrderItemCommand>();
            var orderItemId = await client.CreateOrderOrderItemAsync(command);
            _idTracker["OrderItemId"] = orderItemId;
            return (orderId, orderItemId);
        }

        public async Task<Guid> CreateProduct()
        {
            var client = new ProductsServiceHttpClient(_factory.CreateClient());

            var command = CreateCommand<ProductCreateDto>();
            var productId = await client.CreateProductAsync(command);
            _idTracker["ProductId"] = productId;
            return productId;
        }
    }
}