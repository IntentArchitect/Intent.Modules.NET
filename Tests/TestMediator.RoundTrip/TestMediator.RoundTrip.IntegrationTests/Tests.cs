using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;
using PublisherDb = TestMediator.Publisher.Infrastructure.Persistence.ApplicationDbContext;
using SubscriberDb = TestMediator.Subscriber.Infrastructure.Persistence.ApplicationDbContext;

namespace TestMediator.RoundTrip.IntegrationTests;

public sealed class AppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _database;

    public AppFactory(string database) => _database = database;

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
                ["RabbitMq:Username"] = "guest",
                ["RabbitMq:Password"] = "guest",
                ["MassTransit:RetryInterval:RetryCount"] = "2",
                ["MassTransit:RetryInterval:Interval"] = "00:00:00.500",
            });
        });
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
            }));
    }
}

public sealed class Fixture : IAsyncLifetime
{
    private RabbitMqContainer? _broker;
    private MsSqlContainer? _sqlServer;

    public string PublisherDatabase { get; private set; } = null!;
    public AppFactory<TestMediator.Publisher.Api.Program> Publisher { get; private set; } = null!;
    public AppFactory<TestMediator.Subscriber.Api.Program> Subscriber { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        if (!await IsPortOpenAsync("localhost", 5672))
        {
            _broker = new RabbitMqBuilder("rabbitmq:3.13-management")
                .WithUsername("guest")
                .WithPassword("guest")
                .WithPortBinding(5672, 5672)
                .Build();
            await _broker.StartAsync();
        }

        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();

        string Database(string name) => new SqlConnectionStringBuilder(_sqlServer.GetConnectionString()) { InitialCatalog = name }.ConnectionString;
        PublisherDatabase = Database("RoundTripPublisher");
        var subscriberDatabase = Database("RoundTripSubscriber");

        await using (var db = new PublisherDb(new DbContextOptionsBuilder<PublisherDb>().UseSqlServer(PublisherDatabase).Options, null!))
        {
            await db.Database.EnsureCreatedAsync();
        }

        await using (var db = new SubscriberDb(new DbContextOptionsBuilder<SubscriberDb>().UseSqlServer(subscriberDatabase).Options))
        {
            await db.Database.EnsureCreatedAsync();
        }

        Publisher = new AppFactory<TestMediator.Publisher.Api.Program>(PublisherDatabase);
        Subscriber = new AppFactory<TestMediator.Subscriber.Api.Program>(subscriberDatabase);
        _ = Subscriber.CreateClient();
        _ = Publisher.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Publisher.DisposeAsync();
        await Subscriber.DisposeAsync();
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

public sealed record ShipmentRow(Guid Id, Guid OrderId);

/// <summary>
/// Spike S5/S6, for real: an order created in Publisher reaches Subscriber over the broker, Subscriber's integration
/// handler dispatches a command that creates a shipment, and Publisher's own integration handler dispatches a command
/// that marks the order shipped. Every hop goes through the generated Mediator pipeline and the real MassTransit consumer.
/// </summary>
public class RoundTripScenarios : IClassFixture<Fixture>
{
    private readonly Fixture _fixture;

    public RoundTripScenarios(Fixture fixture) => _fixture = fixture;

    [Fact]
    public async Task OrderCreatedInPublisher_ProducesAShipmentInSubscriber_AndMarksTheOrderShipped()
    {
        var created = await _fixture.Publisher.CreateClient().PostAsJsonAsync("api/orders", new { customerName = "Round", total = 3m });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var orderId = await created.Content.ReadFromJsonAsync<Guid>();

        var shipments = _fixture.Subscriber.CreateClient();
        var shipment = await PollAsync(async () =>
            (await shipments.GetFromJsonAsync<List<ShipmentRow>>("api/shipments"))!.FirstOrDefault(s => s.OrderId == orderId));
        Assert.NotNull(shipment);

        var shipped = await PollAsync(async () => await OrderStatusAsync(orderId) == 1 ? (object?)true : null);
        Assert.NotNull(shipped);
    }

    // OrderStatus.Shipped is the second member; read the column directly because the order query is a secured endpoint.
    private async Task<int> OrderStatusAsync(Guid orderId)
    {
        await using var connection = new SqlConnection(_fixture.PublisherDatabase);
        await connection.OpenAsync();
        await using var command = new SqlCommand("SELECT [Status] FROM [Orders] WHERE [Id] = @id", connection);
        command.Parameters.AddWithValue("@id", orderId);
        var value = await command.ExecuteScalarAsync();
        return value is null or DBNull ? -1 : Convert.ToInt32(value);
    }

    private static async Task<T?> PollAsync<T>(Func<Task<T?>> probe) where T : class
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            var result = await probe();
            if (result is not null)
            {
                return result;
            }

            await Task.Delay(250);
        }

        return null;
    }
}
