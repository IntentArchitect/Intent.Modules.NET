using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Customers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Farmers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Optionals;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ParentWithAnemicChildren;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Basics;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Farmers;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Optionals;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.PagingTS;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren;
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

        public async Task<Guid> CreateFarmer()
        {
            var client = new FarmersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateFarmerCommand>();
            var farmerId = await client.CreateFarmerAsync(command);
            _idTracker["FarmerId"] = farmerId;
            return farmerId;
        }

        public async Task<Guid> CreateMachinesDependencies()
        {
            var farmerId = await CreateFarmer();
            return farmerId;
        }

        public async Task<(Guid FarmerId, Guid MachinesId)> CreateMachines()
        {
            var farmerId = await CreateMachinesDependencies();

            var client = new FarmersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateMachinesCommand>();
            var machinesId = await client.CreateMachinesAsync(farmerId, command);
            _idTracker["MachinesId"] = machinesId;
            return (farmerId, machinesId);
        }

        public async Task<Guid> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }

        public async Task<Guid> CreateBasic()
        {
            var client = new BasicsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateBasicCommand>();
            var basicId = await client.CreateBasicAsync(command);
            _idTracker["BasicId"] = basicId;
            return basicId;
        }

        public async Task<Guid> CreateOptional()
        {
            var client = new OptionalsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateOptionalCommand>();
            var optionalId = await client.CreateOptionalAsync(command);
            _idTracker["OptionalId"] = optionalId;
            return optionalId;
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

        public async Task<Guid> CreateParentWithAnemicChild()
        {
            var client = new ParentWithAnemicChildrenHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateParentWithAnemicChildCommand>();
            var parentWithAnemicChildId = await client.CreateParentWithAnemicChildAsync(command);
            _idTracker["ParentWithAnemicChildId"] = parentWithAnemicChildId;
            return parentWithAnemicChildId;
        }

        public async Task<Guid> CreateCountry()
        {
            var client = new CountriesServiceHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCountryDto>();
            var countryId = await client.CreateCountryAsync(command);
            _idTracker["CountryId"] = countryId;
            return countryId;
        }

        public async Task<Guid> CreateStateDependencies()
        {
            var countryId = await CreateCountry();
            return countryId;
        }

        public async Task<(Guid CountryId, Guid StateId)> CreateState()
        {
            var countryId = await CreateStateDependencies();

            var client = new CountriesServiceHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateStateDto>();
            var stateId = await client.CreateStateAsync(countryId, command);
            _idTracker["StateId"] = stateId;
            return (countryId, stateId);
        }

        public async Task<(Guid CountryId, Guid StateId)> CreateCityDependencies()
        {
            var ids = await CreateState();
            return ids;
        }

        public async Task<(Guid CountryId, Guid StateId, Guid CityId)> CreateCity()
        {
            var ids = await CreateCityDependencies();

            var client = new CountriesServiceHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCityDto>();
            var cityId = await client.CreateCityAsync(ids.CountryId, ids.StateId, command);
            _idTracker["CityId"] = cityId;
            return (ids.CountryId, ids.StateId, cityId);
        }

        public async Task<Guid> CreatePagingTS()
        {
            var client = new PagingTSServiceHttpClient(_factory.CreateClient());

            var command = CreateCommand<PagingTSCreateDto>();
            var pagingTSId = await client.CreatePagingTSAsync(command);
            _idTracker["PagingTSId"] = pagingTSId;
            return pagingTSId;
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