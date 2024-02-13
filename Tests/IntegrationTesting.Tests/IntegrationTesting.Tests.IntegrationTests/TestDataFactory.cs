using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures;
using IntegrationTesting.Tests.IntegrationTests.Services.Children;
using IntegrationTesting.Tests.IntegrationTests.Services.Customers;
using IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns;
using IntegrationTesting.Tests.IntegrationTests.Services.Orders;
using IntegrationTesting.Tests.IntegrationTests.Services.Parents;
using IntegrationTesting.Tests.IntegrationTests.Services.PartialCruds;
using IntegrationTesting.Tests.IntegrationTests.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests
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

        public async Task<Guid> CreateBadSignatures()
        {
            var client = new BadSignaturesHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateBadSignaturesCommand>();
            var badSignaturesId = await client.CreateBadSignaturesAsync(command);
            _idTracker["BadSignaturesId"] = badSignaturesId;
            return badSignaturesId;
        }

        public async Task CreateChildDependencies()
        {
            await CreateParent();
        }

        public async Task<Guid> CreateChild()
        {
            await CreateChildDependencies();

            var client = new ChildrenHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateChildCommand>();
            var childId = await client.CreateChildAsync(command);
            _idTracker["ChildId"] = childId;
            return childId;
        }

        public async Task<Guid> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }

        public async Task<Guid> CreateDtoReturn()
        {
            var client = new DtoReturnsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateDtoReturnCommand>();
            var dto = await client.CreateDtoReturnAsync(command);
            var dtoReturnId = dto.Id;
            _idTracker["DtoReturnId"] = dtoReturnId;
            return dtoReturnId;
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

        public async Task<Guid> CreateParent()
        {
            var client = new ParentsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateParentCommand>();
            var parentId = await client.CreateParentAsync(command);
            _idTracker["ParentId"] = parentId;
            _idTracker["MyParentId"] = parentId;
            return parentId;
        }

        public async Task<Guid> CreatePartialCrud()
        {
            var client = new PartialCrudsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreatePartialCrudCommand>();
            var partialCrudId = await client.CreatePartialCrudAsync(command);
            _idTracker["PartialCrudId"] = partialCrudId;
            return partialCrudId;
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