using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.BadSignatures;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Brands;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Children;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Clients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Customers;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DiffIds;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DtoReturns;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Orders;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Parents;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.PartialCruds;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.RichProducts;
using IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures;
using IntegrationTesting.Tests.IntegrationTests.Services.Brands;
using IntegrationTesting.Tests.IntegrationTests.Services.Children;
using IntegrationTesting.Tests.IntegrationTests.Services.Clients;
using IntegrationTesting.Tests.IntegrationTests.Services.Customers;
using IntegrationTesting.Tests.IntegrationTests.Services.DiffIds;
using IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns;
using IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields;
using IntegrationTesting.Tests.IntegrationTests.Services.Orders;
using IntegrationTesting.Tests.IntegrationTests.Services.Parents;
using IntegrationTesting.Tests.IntegrationTests.Services.PartialCruds;
using IntegrationTesting.Tests.IntegrationTests.Services.Products;
using IntegrationTesting.Tests.IntegrationTests.Services.RichProducts;
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
            fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
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

        public async Task<Guid> CreateBrand()
        {
            var client = new BrandsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateBrandCommand>();
            var brandId = await client.CreateBrandAsync(command);
            _idTracker["BrandId"] = brandId;
            return brandId;
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

        public async Task<Guid> CreateClient()
        {
            var client = new ClientsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateClientCommand>();
            var clientId = await client.CreateClientAsync(command);
            _idTracker["ClientId"] = clientId;
            return clientId;
        }

        public async Task<Guid> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }

        public async Task<Guid> CreateDiffId()
        {
            var client = new DiffIdsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateDiffIdCommand>();
            var diffIdId = await client.CreateDiffIdAsync(command);
            _idTracker["DiffIdId"] = diffIdId;
            _idTracker["MyId"] = diffIdId;
            return diffIdId;
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

        public async Task<Guid> CreateHasDateOnlyField()
        {
            var client = new HasDateOnlyFieldsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateHasDateOnlyFieldCommand>();
            var hasDateOnlyFieldId = await client.CreateHasDateOnlyFieldAsync(command);
            _idTracker["HasDateOnlyFieldId"] = hasDateOnlyFieldId;
            return hasDateOnlyFieldId;
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

        public async Task CreateRichProductDependencies()
        {
            await CreateBrand();
        }

        public async Task<Guid> CreateRichProduct()
        {
            await CreateRichProductDependencies();

            var client = new RichProductsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateRichProductCommand>();
            var richProductId = await client.CreateRichProductAsync(command);
            _idTracker["RichProductId"] = richProductId;
            return richProductId;
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