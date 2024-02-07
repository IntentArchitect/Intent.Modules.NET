# Intent.AspNetCore.IntegrationTesting

This modules adds Integration Testing for ASP.NET core applications with container support for database (MS MSQL, PostGres, CosmosDB).

## What is Integration Testing

Integration testing is a phase in the software development life cycle where individual software modules are combined and tested as a group. The purpose of integration testing is to ensure that the different components of a software application work together as expected when integrated. This is done to identify and address any issues that may arise when multiple modules interact with each other.

This module adds an xUnit testing project to you ASP.NET Core application which contains Integrations Tests which can be run to validate your application is working end-to-end against containerized infrastructure like databases e.g. `MS SQL Server`, `Postgres` or `CosmosDB Emulator`. These tests do not replace Unit testing but rather compliment it ensure the individually tested pieces work together correctly.

This module uses `Test.containers` to spin up and host infrastructure in docker containers.

For more information on Test.containers read the official [documentation](https://testcontainers.com/).

## Module Settings

This modules has the following settings.

![Integration Testing Settings](./docs/images/integration-test-settings.png)

### Container Isolation

This setting determines the default container isolation level for your test.

- `Shared Container`, the tests share a container, i.e. 1 database container is spun up and all Test Class run against this container
- `Container per Test Class`, Each Test Class spins up a new container to execute it's tests against.

## What's in this module?

This module consumes your `Exposed HTTP Endpoints`, in the `Service Designer` and generates the following implementation:-

- Adds Integration xUnit Testing project.
- Generates service proxies for all service end points, to use to interact with the Application under test.
- Add container support for `MS SQL Server`, `Postgres` and `CosmosDB`
- Generates test classes for each service end point.

## Testing Isolation

The default isolation can be configured with the following implications :

`Shared Container` is significantly more performant but the database state is not reset between tests, so tests either need to be ok with this or have to clean the data themselves

`Container per Test Class` is slower, but each Test Class runs against a newly created container.

If you are running `Shared Container`, you can set up specific Test Class's to require a Clean Container. This hybrid model can give you the best of both worlds. To setup such a test ensure the Test Class implements `IClassFixture<IntegrationTestWebAppFactory>` and remove the `Collection("SharedContainer")` attribute.

```csharp

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IsolatedTests : BaseIntegrationTest, IClassFixture<IntegrationTestWebAppFactory>
    {
        public CustomerServiceTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
        ...
    }
```
