using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using MassTransit;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;
using Xunit.Abstractions;
using TestMediator.Subscriber.Application.Common.Behaviours;
using TestMediator.Subscriber.Application.Shipments;
using TestMediator.Subscriber.Application.Shipments.CreateShipment;
using TestMediator.Subscriber.Application.Shipments.GetShipments;
using TestMediator.Subscriber.Infrastructure.Persistence;
using PublisherMessages = TestMediator.Publisher.Eventing.Messages;
using SubscriberMessages = TestMediator.Subscriber.Eventing.Messages;

namespace TestMediator.Subscriber.IntegrationTests;

public sealed class SubscriberFactory : WebApplicationFactory<TestMediator.Subscriber.Api.Program>
{
    private readonly string _database;

    public SubscriberFactory(string database) => _database = database;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _database,
                ["RabbitMq:Host"] = "localhost",
                ["RabbitMq:VirtualHost"] = "/",
                ["RabbitMq:Username"] = Fixture.BrokerUser,
                ["RabbitMq:Password"] = Fixture.BrokerPassword,
                ["MassTransit:RetryInterval:RetryCount"] = "2",
                ["MassTransit:RetryInterval:Interval"] = "00:00:00.500",
            });
        });
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
        {
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
            });
        });
    }
}

/// <summary>Starts (or reuses) RabbitMQ on localhost:5672 and a SQL Server container, then boots the Subscriber in-process.</summary>
public sealed class Fixture : IAsyncLifetime
{
    public const string BrokerUser = "guest";
    public const string BrokerPassword = "guest";

    private RabbitMqContainer? _broker;
    private MsSqlContainer? _sqlServer;

    public SubscriberFactory App { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        if (!await IsPortOpenAsync("localhost", 5672))
        {
            _broker = new RabbitMqBuilder("rabbitmq:3.13-management")
                .WithUsername(BrokerUser)
                .WithPassword(BrokerPassword)
                .WithPortBinding(5672, 5672)
                .Build();
            await _broker.StartAsync();
        }

        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();
        var database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "TestMediatorSubscriber"
        }.ConnectionString;

        await using (var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(database).Options))
        {
            await db.Database.EnsureCreatedAsync();
        }

        App = new SubscriberFactory(database);
        _ = App.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
        if (_broker is not null)
        {
            await _broker.DisposeAsync();
        }

        if (_sqlServer is not null)
        {
            await _sqlServer.DisposeAsync();
        }
    }

    private static async Task<bool> IsPortOpenAsync(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var connect = client.ConnectAsync(host, port);
            return await Task.WhenAny(connect, Task.Delay(500)) == connect && client.Connected;
        }
        catch
        {
            return false;
        }
    }
}

[CollectionDefinition(Name)]
public sealed class FixtureCollection : ICollectionFixture<Fixture>
{
    public const string Name = "subscriber";
}

/// <summary>The defaults pipeline: the opt-in behaviours are off and nothing is secured, so there is no Authorization item.</summary>
public class PipelineShapeChecks
{
    [Fact]
    public void DefaultsPipeline_CommandsGetFlushValidationAndUnitOfWork_QueriesSkipUnitOfWork()
    {
        var services = new ServiceCollection();
        TestMediator.Subscriber.Infrastructure.DependencyInjection.AddInfrastructure(services, new ConfigurationBuilder().Build());

        Type[] PipelineOf<TMessage, TResponse>() where TMessage : notnull, IMessage =>
            services.Where(d => d.ServiceType == typeof(IPipelineBehavior<TMessage, TResponse>))
                .Select(d => d.ImplementationType!.GetGenericTypeDefinition()).ToArray();

        Assert.Equal(
            new[] { typeof(MessageBusPublishBehaviour<,>), typeof(ValidationBehaviour<,>), typeof(UnitOfWorkBehaviour<,>) },
            PipelineOf<CreateShipmentCommand, Guid>());
        Assert.Equal(
            new[] { typeof(MessageBusPublishBehaviour<,>), typeof(ValidationBehaviour<,>) },
            PipelineOf<GetShipmentsQuery, List<ShipmentDto>>());
    }
}

[Collection(FixtureCollection.Name)]
public class SubscriberScenarios
{
    private readonly Fixture _fixture;
    private readonly ITestOutputHelper _output;

    public SubscriberScenarios(Fixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task FastEndpoints_CommandAndQuery_RoundTripThroughDispatcher()
    {
        var client = _fixture.App.CreateClient();
        var orderId = Guid.NewGuid();

        var created = await client.PostAsJsonAsync("api/shipments", new { orderId });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var shipmentId = await created.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, shipmentId);

        var all = await client.GetFromJsonAsync<List<ShipmentDto>>("api/shipments");
        Assert.Contains(all!, s => s.Id == shipmentId && s.OrderId == orderId);
    }

    // Spike S9: FastEndpoints dispatch maps a validation failure to 400, and the handler never runs.
    [Fact]
    public async Task FastEndpoints_InvalidCommand_Returns400_AndCreatesNothing()
    {
        var client = _fixture.App.CreateClient();
        var before = await client.GetFromJsonAsync<List<ShipmentDto>>("api/shipments");

        var response = await client.PostAsJsonAsync("api/shipments", new { orderId = Guid.Empty });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var after = await client.GetFromJsonAsync<List<ShipmentDto>>("api/shipments");
        Assert.Equal(before!.Count, after!.Count);
    }

    private async Task<T> ObserveAsync<T>(Func<IBus, Task> trigger, Func<T, bool> match, TimeSpan wait) where T : class
    {
        var bus = _fixture.App.Services.GetRequiredService<IBus>();
        var arrival = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var observer = bus.ConnectReceiveEndpoint($"test-{typeof(T).Name}-{Guid.NewGuid():N}", endpoint =>
            endpoint.Handler<T>(context =>
            {
                if (match(context.Message))
                {
                    arrival.TrySetResult(context.Message);
                }

                return Task.CompletedTask;
            }));
        await observer.Ready;
        try
        {
            await trigger(bus);
            var completed = await Task.WhenAny(arrival.Task, Task.Delay(wait));
            Assert.True(completed == arrival.Task, $"{typeof(T).Name} was not delivered within {wait}.");
            return await arrival.Task;
        }
        finally
        {
            await observer.StopAsync();
        }
    }

    [Fact]
    public async Task InvokeOnlyHandler_DispatchesTheCommand_AndItsPublishIsFlushedByTheConsumer()
    {
        var orderId = Guid.NewGuid();

        var published = await ObserveAsync<SubscriberMessages.ShipmentCreatedEvent>(
            bus => bus.Publish(new PublisherMessages.OrderCreatedEvent { OrderId = orderId, CustomerName = "Mallory" }),
            message => message.OrderId == orderId,
            TimeSpan.FromSeconds(20));

        // The nested command's publish was only released after the shipment row committed.
        using var scope = _fixture.App.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.True(await db.Shipments.AnyAsync(s => s.Id == published.ShipmentId && s.OrderId == orderId));
    }

    [Fact]
    public async Task RelayHandler_PublishesTheMappedMessage()
    {
        var orderId = Guid.NewGuid();

        var relayed = await ObserveAsync<SubscriberMessages.OrderRelayedEvent>(
            bus => bus.Publish(new PublisherMessages.OrderCancelledEvent { OrderId = orderId }),
            message => message.OrderId == orderId,
            TimeSpan.FromSeconds(20));

        Assert.Equal(orderId, relayed.OrderId);
    }

    [Fact]
    public async Task MixedHandler_InvokesTheCommand_AndPublishesTheMappedMessage()
    {
        var orderId = Guid.NewGuid();

        var dispatched = await ObserveAsync<SubscriberMessages.ShipmentDispatchedEvent>(
            bus => bus.Publish(new PublisherMessages.OrderShippedEvent { OrderId = orderId }),
            message => message.OrderId == orderId,
            TimeSpan.FromSeconds(20));

        Assert.Equal(orderId, dispatched.OrderId);
    }
}
