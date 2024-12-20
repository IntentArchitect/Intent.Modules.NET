# Intent.AspNetCore.IntegrationTesting.CRUD

This module provides Integration Test implementation for Services which are CRUD orientated.

## What's in this module?

This module consumes your `Services` both `ServiceModel` and `CQRS`, in the `Service Designer` and generates an Integration test for each Endpoint.

For a service to be eligible is must implement at least:

- Create{Entity} - returning either the `Primary Key` or a `DTO` with a mapped `Primary Key`.
- Get{Entity}ById - returning an `Entity` based `DTO` taking a single parameter of the `Entitiy`s primary key.

## Note on Cosmos DB Integration Testing

The CosmosDB Emulator for Linux does not run consistently using MS-hosted agents/runners (Azure DevOps or GitHub). Here (Azure/azure-cosmos-db-emulator-docker#45.) 
Cosmos related tests have the following Trait applied `[Trait("Requirement", "CosmosDB")]`, you can ignore these in your CI/CD pipeline as follows:-

```
dotnet test --filter Category!=CosmosDB
```


## Sample Tests

```csharp
        [Fact]
        public async Task CreateCustomer_ShouldCreateCustomer()
        {
            //Arrange
            var client = new CustomersServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateCustomerDependencies();

            var command = dataFactory.CreateCommand<CustomerCreateDto>();

            //Act
            var customerId = await client.CreateCustomerAsync(command);

            //Assert
            var customer = await client.FindCustomerByIdAsync(customerId);
            Assert.NotNull(customer);
        }
```

```csharp
        [Fact]
        public async Task FindCustomerById_ShouldFindCustomerById()
        {
            //Arrange
            var client = new CustomersServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            //Act
            var customer = await client.FindCustomerByIdAsync(customerId);

            //Assert
            Assert.NotNull(customer);
        }
```

```csharp
        [Fact]
        public async Task UpdateCustomer_ShouldUpdateCustomer()
        {
            //Arrange
            var client = new CustomersServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            var command = dataFactory.CreateCommand<CustomerUpdateDto>();
            command.Id = customerId;

            //Act
            await client.UpdateCustomerAsync(customerId, command);

            //Assert
            var customer = await client.FindCustomerByIdAsync(customerId);
            Assert.NotNull(customer);
            Assert.Equal(command.Name, customer.Name);
        }
```

```csharp
        [Fact]
        public async Task DeleteCustomer_ShouldDeleteCustomer()
        {
            //Arrange
            var client = new CustomersServiceHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            //Act
            await client.DeleteCustomerAsync(customerId);

            //Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.FindCustomerByIdAsync(customerId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
```
