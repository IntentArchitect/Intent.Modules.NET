using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Customers;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.ExternalDocs;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Products;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Customers;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.ExternalDocs;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Orders;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests
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

        public async Task<string> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }

        [IntentMerge]
        public async Task<long> CreateExternalDoc()
        {
            var client = new ExternalDocsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateExternalDocCommand>();
            command.Id = UniqueLongGenerator.GetNextId();
            var externalDocId = await client.CreateExternalDocAsync(command);
            _idTracker["ExternalDocId"] = externalDocId;
            return externalDocId;
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

        public async Task<string> CreateProduct()
        {
            var client = new ProductsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateProductCommand>();
            var productId = await client.CreateProductAsync(command);
            _idTracker["ProductId"] = productId;
            return productId;
        }
    }
}