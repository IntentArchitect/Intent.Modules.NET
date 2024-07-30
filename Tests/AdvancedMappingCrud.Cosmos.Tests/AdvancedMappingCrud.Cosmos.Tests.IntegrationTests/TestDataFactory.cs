using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.BasicOrderBies;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Customers;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.ExplicitETags;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Products;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.SimpleOdata;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Products;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests
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

        public async Task<string> CreateBasicOrderBy()
        {
            var client = new BasicOrderBiesHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateBasicOrderByCommand>();
            var basicOrderById = await client.CreateBasicOrderByAsync(command);
            _idTracker["BasicOrderById"] = basicOrderById;
            return basicOrderById;
        }

        public async Task<string> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }

        public async Task<string> CreateExplicitETag()
        {
            var client = new ExplicitETagsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateExplicitETagCommand>();
            var explicitETagId = await client.CreateExplicitETagAsync(command);
            _idTracker["ExplicitETagId"] = explicitETagId;
            return explicitETagId;
        }

        public async Task CreateOrderDependencies()
        {
            await CreateCustomer();
            await CreateProduct();
        }

        public async Task<string> CreateOrder()
        {
            await CreateOrderDependencies();

            var client = new OrdersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateOrderCommand>();
            var orderId = await client.CreateOrderAsync(command);
            _idTracker["OrderId"] = orderId;
            return orderId;
        }

        public async Task<string> CreateOrderItemDependencies()
        {
            var orderId = await CreateOrder();
            await CreateProduct();
            return orderId;
        }

        public async Task<(string OrderId, string OrderItemId)> CreateOrderItem()
        {
            var orderId = await CreateOrderItemDependencies();

            var client = new OrdersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateOrderOrderItemCommand>();
            var orderItemId = await client.CreateOrderOrderItemAsync(command);
            _idTracker["OrderItemId"] = orderItemId;
            return (orderId, orderItemId);
        }

        public async Task<string> CreateProduct()
        {
            var client = new ProductsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateProductCommand>();
            var productId = await client.CreateProductAsync(command);
            _idTracker["ProductId"] = productId;
            return productId;
        }

        public async Task<string> CreateSimpleOdata()
        {
            var client = new SimpleOdataHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateSimpleOdataCommand>();
            var simpleOdataId = await client.CreateSimpleOdataAsync(command);
            _idTracker["SimpleOdataId"] = simpleOdataId;
            return simpleOdataId;
        }
    }
}