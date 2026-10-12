using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;
using PublisherDbContext = CqrsSpike.Publisher.Infrastructure.Persistence.ApplicationDbContext;
using SubscriberDbContext = CqrsSpike.Subscriber.Infrastructure.Persistence.ApplicationDbContext;

namespace CqrsSpike.Scenarios.Infrastructure;

/// <summary>
/// Starts (or reuses) a RabbitMQ broker on localhost:5672, starts a SQL Server container with one
/// database per app, and boots BOTH applications in-process against them. One instance is shared by every scenario so that no two hosts compete for the same
/// MassTransit queues.
/// </summary>
public sealed class SpikeFixture : IAsyncLifetime
{
    public const string BrokerUser = "guest";
    public const string BrokerPassword = "guest";

    private RabbitMqContainer? _container;
    private MsSqlContainer? _sqlServer;

    public string PublisherDatabase { get; private set; } = null!;
    public string SubscriberDatabase { get; private set; } = null!;

    public PublisherFactory Publisher { get; private set; } = null!;
    public SubscriberFactory Subscriber { get; private set; } = null!;
    public PipelineRecorder Recorder { get; } = new();

    public async Task InitializeAsync()
    {
        if (!await IsPortOpenAsync("localhost", 5672))
        {
            _container = new RabbitMqBuilder("rabbitmq:3.13-management")
                .WithUsername(BrokerUser)
                .WithPassword(BrokerPassword)
                .WithPortBinding(5672, 5672)
                .Build();
            await _container.StartAsync();
        }

        // EF Core InMemory ignores transactions and unique indexes; SQL Server enforces both.
        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();
        PublisherDatabase = SqlServerDatabase.For(_sqlServer.GetConnectionString(), "CqrsSpikePublisher");
        SubscriberDatabase = SqlServerDatabase.For(_sqlServer.GetConnectionString(), "CqrsSpikeSubscriber");
        await using (var db = new PublisherDbContext(
            new DbContextOptionsBuilder<PublisherDbContext>().UseSqlServer(PublisherDatabase).Options, null!))
        {
            await db.Database.EnsureCreatedAsync();
        }

        await using (var db = new SubscriberDbContext(
            new DbContextOptionsBuilder<SubscriberDbContext>().UseSqlServer(SubscriberDatabase).Options))
        {
            await db.Database.EnsureCreatedAsync();
        }

        Publisher = new PublisherFactory(Recorder, PublisherDatabase);
        Subscriber = new SubscriberFactory(SubscriberDatabase);

        // Start both hosts before any scenario runs. MassTransitHostOptions.WaitUntilStarted (see
        // FactoryConfiguration) makes each start block until its bus is connected.
        _ = Publisher.CreateClient();
        _ = Subscriber.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Publisher.DisposeAsync();
        await Subscriber.DisposeAsync();
        if (_container is not null)
        {
            await _container.DisposeAsync();
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
public sealed class SpikeCollection : ICollectionFixture<SpikeFixture>
{
    public const string Name = "spike";
}
